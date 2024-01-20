using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SelectedItemColorNotSetDatabase;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SelectedItemColorNotSet.ViewModels
{
    public partial class CustomPopupViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, INotifyPropertyChanged
    {
        [ObservableProperty]
        public ObservableRangeCollection<SavedStore> savedStores;

        [ObservableProperty]
        public ObservableRangeCollection<GroupedSavedStoreItemList> groupedSavedStoreItems;

        [ObservableProperty]
        public SavedStore selectedSavedStore;



        public AsyncCommand<bool> RefreshCommand;
        public AsyncCommand<SavedStore> DeleteSavedStoreCommand;
        public AsyncCommand OkButtonCommand { get; }



        public string SelectedSavedStoreName { get; set; }

        

                
        public CustomPopupViewModel()
        {
            SavedStores = new ObservableRangeCollection<SavedStore>();

            RefreshCommand = new AsyncCommand<bool>(RefreshDatabase);
            DeleteSavedStoreCommand = new AsyncCommand<SavedStore>(DeleteSavedStore);
            OkButtonCommand = new AsyncCommand(OnOkButtonCommand);

            RefreshCommand.ExecuteAsync(true);
        }

        private List<SavedStore> GetSomeStoreExamples()
        {
            var savedStore1 = new SavedStore()
            {
                Name = "Apple Store",
                LastSavedDate = DateTime.Now,
            };

            var savedStore2 = new SavedStore()
            {
                Name = "Banana Store",
                LastSavedDate = DateTime.Now,
            };

            return new List<SavedStore> { savedStore1, savedStore2 };
        }

        private async Task RefreshDatabase(bool selectSavedStore = true)
        {
            var savedStores = GetSomeStoreExamples();
            
            ClearSavedStores();
            SavedStores.AddRange(savedStores);

            GroupSavedStoresWithSearchFilter();       // Initialize the search filter with empty values to retrieve all items

            if (selectSavedStore)
            {
                // If a name for a store is provided, attempt to select it;
                // otherwise, select the element with Index = 0.
                await SelectSavedStoreByName(SelectedSavedStoreName);
            }

            await Task.CompletedTask;
        }

        private void ClearSavedStores()
        {
            int numSavedStores = SavedStores.Count;
            for (int i = numSavedStores - 1; i >= 0; i--)
            {
                SavedStores.RemoveAt(i);
            }

            SavedStores.ReplaceRange(new List<SavedStore>());

            GC.Collect();
        }

        private async Task SelectSavedStoreByName(string savedStoreName)
        {
            if (string.IsNullOrWhiteSpace(savedStoreName))
            {
                await SelectSavedStoreByIndex(0);
                return;
            }

            SavedStore foundSavedStore = GroupedSavedStoreItems.SelectMany(list => list).FirstOrDefault(savedStore => savedStore.Name == savedStoreName);

            SelectedSavedStore = foundSavedStore;

            await Task.CompletedTask;
        }

        private async Task SelectSavedStoreByIndex(int index)
        {
            if (SavedStores.Count == 0)
            {
                SelectedSavedStore = null;
            }
            else if (SavedStores.Count >= 1)
            {
                // If there is at least one element left and we intend
                // to select an index outside the valid range, 
                // set it to the corresponding lower or upper range value
                if (index < 0)
                {
                    index = 0;
                }
                else if (index >= SavedStores.Count)
                {
                    index = SavedStores.Count - 1;
                }

                var selectSuccessor = SavedStores[index];
                await SelectSavedStoreByName(selectSuccessor.Name);
            }

            await Task.CompletedTask;
        }

        private async Task DeleteSavedStore(SavedStore storeToDelete)
        {
            var indexOfStoreToDelete = SavedStores.IndexOf(storeToDelete);
            if (indexOfStoreToDelete != -1)
            {
                await RefreshDatabase(false);

                await SelectSavedStoreByIndex(indexOfStoreToDelete - 1);
            }

            await Task.CompletedTask;
        }

        #region Search Filter

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    OnSearchTextChanged();
                }
            }
        }

        protected virtual void OnSearchTextChanged()
        {
            GroupSavedStoresWithSearchFilter();
        }

        #endregion

        #region Grouping of SavedStores

        private void GroupSavedStoresWithSearchFilter()
        {
            IOrderedEnumerable<KeyValuePair<string, List<SavedStore>>> groupedSavedStoreItemsDictionary;
            if (string.IsNullOrEmpty(SearchText))
            {
                groupedSavedStoreItemsDictionary = SavedStores
                    .OrderBy(savedStoreItem => savedStoreItem.Name)
                    .GroupBy(savedStoreItem => savedStoreItem.Name.ToUpperInvariant().Substring(0, 1))
                    .ToDictionary(group => group.Key, group => group.ToList()).OrderBy(group => group.Key);
            }
            else
            {
                groupedSavedStoreItemsDictionary = SavedStores
                    .Where(savedStore => savedStore.Name.ToLower().StartsWith(SearchText.ToLower()))
                    .OrderBy(savedStoreItem => savedStoreItem.Name)
                    .GroupBy(savedStoreItem => savedStoreItem.Name.ToUpperInvariant().Substring(0, 1))
                    .ToDictionary(group => group.Key, group => group.ToList()).OrderBy(group => group.Key);
            }

            GroupedSavedStoreItems = new ObservableRangeCollection<GroupedSavedStoreItemList>();

            foreach (KeyValuePair<string, List<SavedStore>> item in groupedSavedStoreItemsDictionary)
            {
                GroupedSavedStoreItems.Add(new GroupedSavedStoreItemList(item.Key, new List<SavedStore>(item.Value)));
            }

            OnPropertyChanged(nameof(GroupedSavedStoreItems));
        }

        #endregion

        #region OK Button Command

        private async Task OnOkButtonCommand()
        {
            await Task.CompletedTask;
        }

        #endregion

        #region Eventing

        readonly Microsoft.Maui.WeakEventManager weakEventManager = new();

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => weakEventManager.AddEventHandler(value);
            remove => weakEventManager.RemoveEventHandler(value);
        }

        void OnPropertyChanged([CallerMemberName] in string propertyName = "") => weakEventManager.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));

        #endregion
    }
}
