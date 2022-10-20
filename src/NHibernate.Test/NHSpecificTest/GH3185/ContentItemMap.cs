using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3185
{
	public class ContentItemMap : ClassMapping<ContentItem>
	{
		public ContentItemMap()
		{
			Table("ContentItem");
			Id(x => x.Id, m => m.Generator(Generators.Identity));
			Property(x => x.Name);
			ManyToOne(x => x.Parent, map =>
			{
				map.NotNullable(false);
				map.NotFound(NotFoundMode.Ignore);
				map.Lazy(LazyRelation.Proxy);
				map.Cascade(Mapping.ByCode.Cascade.Persist);
				map.ForeignKey("none");
			});

			ManyToOne(x => x.Category, map =>
			{
				map.NotNullable(false);
				map.NotFound(NotFoundMode.Ignore);
				map.Lazy(LazyRelation.Proxy);
				map.Cascade(Mapping.ByCode.Cascade.Persist);
				map.ForeignKey("none");
			});
		}
	}
}
