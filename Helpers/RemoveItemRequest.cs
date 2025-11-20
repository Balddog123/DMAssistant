using DMAssistant.Model;

namespace DMAssistant.Helpers
{
    public class RemoveItemRequest
    {
        public PlayerCharacter Player { get; set; }
        public Item Item { get; set; }
    }
}
