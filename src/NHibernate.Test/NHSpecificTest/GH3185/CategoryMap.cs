using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3185
{
	public class CategoryMap : ClassMapping<Category>
	{
		public CategoryMap()
		{
			Table("Category");
			Id(x => x.Id, m => m.Generator(Generators.Identity));
			Property(x => x.Name);
		}
	}
}
