using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot
{
    [Serializable]
    public class SearchParameters
    {
        public string searchCategory { set; get; }
        public string searchType { set; get; }
        public bool searchHasGarage { set; get; }
        public bool searchHasGarden { set; get; }
        public int searchMaxCount { set; get; }

    }
    [Serializable]
    public class SearchEntry : IComparable<string>
    {
        public string searchKey { get; set; }
        public string searchValue { get; set; }
        public bool searchValid { get; set; }
        public SearchChoice searchChoice { get; set; }
        public int CompareTo(string other)
        {
            return this.searchKey == other ? 0 : 1;
        }
        public override string ToString()
        {
            return this.searchValue;
        }
        public static bool operator ==(SearchEntry first,SearchEntry scnd)
        {
            return first.searchKey!=null? first.searchKey == scnd.searchKey:first.searchValue==scnd.searchValue;
        } public static bool operator !=(SearchEntry first,SearchEntry scnd)
        {
            return first.searchKey != scnd.searchKey;
        }
        public static List<SearchEntry> CreateChoiceList()
        {
          return  new List<SearchEntry>() {
                       new SearchEntry { searchKey=SearchChoice.Yes.ToString(),searchValue=Resources.Search.SearchDialog.SearchYes},
                       new SearchEntry { searchKey=SearchChoice.No.ToString(),searchValue=Resources.Search.SearchDialog.SearchNo},
                       new SearchEntry { searchKey=SearchChoice.Both.ToString(),searchValue=Resources.Search.SearchDialog.SearchBoth} };
        }
    }
    [Serializable]
    public class RangeEntry:IComparable<int>
    {
        public int RangeStart { set; get; }
        public int RangeCount { set; get; }

        public int CompareTo(int other)
        {
            return this.RangeCount > other ? 1 : 0;
        }
    }
    [Serializable]
    public enum SearchChoice
    {
        Yes=0,
        No=1,
        Both=2
    }
}