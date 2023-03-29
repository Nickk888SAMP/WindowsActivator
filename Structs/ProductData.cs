namespace WindowsActivator
{
    struct ProductData
    {
        public int Build { private set; get; }
        public string Architecture { private set; get; }
        public string Name { private set; get; }
        public string PFN { private set; get; }
        public string EditionID { private set; get; }

        public ProductData(int Build, string Architecture, string Name, string PFN, string EditionID)
        {
            this.Build = Build;
            this.Architecture = Architecture;
            this.Name = Name;
            this.PFN = PFN;
            this.EditionID = EditionID;
        }
    }
}
