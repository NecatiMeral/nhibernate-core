using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3185
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		const string RootName = "Root";

		int _categoryId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<CategoryMap>();
			mapper.AddMapping<ContentItemMap>();

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var cat1 = new Category { Name = "Category1" };
				session.Save(cat1);

				var rootItem = new ContentItem { Name = RootName, Category = cat1 };
				session.Save(rootItem);

				var item1 = new ContentItem { Name = "Item", Category = cat1, Parent = rootItem };
				session.Save(item1);

				transaction.Commit();

				_categoryId = cat1.Id;
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from ContentItem").ExecuteUpdate();
				session.CreateQuery("delete from Category").ExecuteUpdate();

				transaction.Commit();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void QueryWorks()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<ContentItem>()
					.Where(x => x.Category.Id == _categoryId)
					.Where(x => x.Parent == null || x.Parent.Id == 0)
					.Select(x => x.Name);

				var sql = GetGeneratedSql(query, session);

				Assert.That(query.FirstOrDefault(), Is.EqualTo(RootName));
			}
		}

		public string GetGeneratedSql(IQueryable queryable, ISession session)
		{
			var sessionImp = (ISessionImplementor) session;
			var nhLinqExpression = new NhLinqExpression(queryable.Expression, sessionImp.Factory);
			var translatorFactory = new ASTQueryTranslatorFactory();
			var translators = translatorFactory.CreateQueryTranslators(nhLinqExpression, null, false, sessionImp.EnabledFilters, sessionImp.Factory);

			return translators[0].SQLString;
		}
	}
}
