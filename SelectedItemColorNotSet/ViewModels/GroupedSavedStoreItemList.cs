using SelectedItemColorNotSetDatabase;

namespace SelectedItemColorNotSet.ViewModels
{
    public partial class GroupedSavedStoreItemList : List<SavedStore>
    {
        public string GroupName { get; set; }

        public GroupedSavedStoreItemList(string groupName, List<SavedStore> storeItem) : base(storeItem)
        {
            GroupName = groupName;
        }
    }
}
