public class Item
{
    public int Id { get; set; }
    public bool Interact { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }

    public Item(int id, bool interact, string name, string desc, string slug)
    {
        Id = id;
        Interact = interact;
        Name = name;
        Description = desc;
        Slug = slug;
    }

    public Item()
    {
        Id = -1;
        Interact = false;
        Name = "default";
        Description = "default";
        Slug = "default";
    }
}
