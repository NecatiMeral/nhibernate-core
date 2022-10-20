namespace NHibernate.Test.NHSpecificTest.GH3185
{
	public class ContentItem
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ContentItem Parent { get; set; }
		public virtual Category Category { get; set; }
	}
}
