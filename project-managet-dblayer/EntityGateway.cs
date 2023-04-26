using project_managet_models;

namespace project_managet_dblayer
{
    public partial class EntityGateway
    {
        public ProjectManagerContext Context { get; set; } = new();

        public void AddOrUpdate(params IEntity[] entities)
        {
            var toAdd = entities.Where(x => x.Id == Guid.Empty);
            var toUpdate = entities.Except(toAdd);
            Context.AddRange(toAdd);
            Context.UpdateRange(toUpdate);
            Context.SaveChanges();
        }

        public void Delete(params IEntity[] entities)
        {
            Context.RemoveRange(entities);
            Context.SaveChanges();
        }
    }
}