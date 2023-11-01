namespace HelpDeskTicketing.Models
{
    public class Branch
    {

        public string Id { get; set; }
        public string Name { get; set; }

        public List<AppUser> AppUsers{ get; set; }


    }
}
