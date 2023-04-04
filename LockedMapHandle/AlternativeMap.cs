using System;
using System.Collections.Generic;

namespace MaidRemake.LockedMapHandle
{
    public class AlternativeMap
    {
        private List<string> mapList = new List<string>();

		public AlternativeMap(List<string> mapList)
        {
            this.mapList = mapList;
        }

		private int currIndex = 0;

        public void Init()
        {
            currIndex = 0;
        }

        public int Count()
        {
            return mapList.Count;
        }

        public string GetNext()
        {
            if (mapList.Count > 0)
            {
                if (currIndex >= mapList.Count)
                    currIndex = 0;
                currIndex++;
                return mapList[currIndex - 1];
            }
            return null;
        }
    }
}
