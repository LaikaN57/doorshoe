/*

Filename: getgroups.cs
Author:   Alex Kennedy <alexzanderkennedy@gmail.com>
Date:     20170827
Version:  1.0.0.1

Project:  Door Shoe
Company:  Project Moose

   Copyright 1990-2017 Alex Kennedy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

*/

using System.Collections.Generic;
using System.DirectoryServices;

        /// <summary>
        /// This will find all groups that a user is a member of either directly or indirectly.
        /// 
        /// Notes:
        /// 1) Currently it does not take into account primary group membership.
        /// 2) This does no input validation.
        /// 3) This LDAP query is inefficient because it searches the entire domain.
        /// 4) No exception checking is done.
        /// 5) There is not check to see if the user or AD exists.
        /// </summary>
        /// <param name="distinguishedname">The distinguished name of the user to lookup.</param>
        /// <returns>A list of groups the user is a member of either directly or indirectly.</returns>
        public IEnumerable<string> GetGroups(string distinguishedname)
        {
            DirectoryEntry deRootDSE = new DirectoryEntry("LDAP://RootDSE");
            string sDefaultNamingContext = deRootDSE.Properties["defaultNamingContext"][0].ToString();
            deRootDSE.Dispose();
            deRootDSE = null;
            DirectoryEntry deDefaultNamingContext = new DirectoryEntry("LDAP://" + sDefaultNamingContext);
            sDefaultNamingContext = null;
            string sFilter = "(member:1.2.840.113556.1.4.1941:=" + distinguishedname + ")";
            string[] sProperties = { "distinguishedname" };
            DirectorySearcher dsQuery = new DirectorySearcher(deDefaultNamingContext, sFilter, sProperties, SearchScope.Subtree);
            sFilter = null;
            int iLength = sProperties.Length;
            for (int i = 0; i < iLength; i++)
            {
                sProperties[i] = null;
            }
            sProperties = null;
            iLength = 0; //security
            SearchResultCollection srcResults = dsQuery.FindAll();
            iLength = srcResults.Count; //reuse
            string[] ret = new string[iLength];
            for (int i = 0; i < iLength; i++)
            {
                ret[i] = srcResults[i].Properties["distinguishedname"][0].ToString();
            }
            return ret;
        }