namespace BuildForgeApp.Models
{ // Junction table linking builds and components (many-to-many)
    public class BuildComponent
    {
        public int Id { get; set; }

        public int BuildId { get; set; }
        public Build? Build { get; set; }

        public int PcComponentId { get; set; }
        public PcComponent? PcComponent { get; set; }
    }
}