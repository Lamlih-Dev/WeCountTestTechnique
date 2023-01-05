namespace WeCountRecrutementWebsite.Models
{
    public class Candidate
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int phoneNumber { get; set; }
        public string studiesLevel { get; set; }
        public int yearsOfExperience { get; set; }
        public string lastEmploye { get; set; }
        public DateTime inscriptionDate { get; set; }
        public string cvUrl { get; set; }
    }
}
