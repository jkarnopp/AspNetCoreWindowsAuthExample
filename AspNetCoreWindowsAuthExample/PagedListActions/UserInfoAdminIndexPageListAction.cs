using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.Repositories;
using X.PagedList;

namespace AspNetCoreWindowsAuthExample.PagedListActions
{
    public interface IUserInfoAdminIndexPageListAction
    {
        Task<IPagedList<UserInformation>> GetUserInfos(IUnitOfWork unitOfWork);
    }

    /// <summary>
    /// This class inherits from the PageListAction class and provides the logic to create a filtered, sorted, and paged list. The first set of properties
    /// are associated with sort headings in the view.
    /// </summary>
    public class UserInfoAdminIndexPageListAction : PageListAction, IUserInfoAdminIndexPageListAction
    {
        //Parameter for storing sort option for the view
        public string NameSortParm { get; set; }

        public string FirstNameSortParm { get; set; }

        //Holder for ID Parameter
        public int? UserInformationId { get; set; }

        public string LanId { get; set; }
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// The constructor takes the input parameters and sets the base properties, then alternates the sort headers based on ascending and descending order.
        /// This is also where the page number is set for the list.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        public UserInfoAdminIndexPageListAction(int pageSize, string sortOrder, string currentFilter, string searchString, int? page, int? id)
            : base(pageSize, sortOrder, currentFilter, searchString, page)
        {
            //_unitOfWork = unitOfWork;
            //Set the alternate parameter text to handle switching from ascending to descending
            NameSortParm = String.IsNullOrEmpty(SortOrder) ? "Name_desc" : ""; //being empty calls the default
            FirstNameSortParm = SortOrder == "FirstName" ? "FirstName_desc" : "FirstName";

            PageNumber = (page ?? 1);

            this.UserInformationId = id;
        }

        /// <summary>
        /// This method retrieves the filtered and sorted data from the database and returns a sized pagelist.
        /// </summary>
        /// <returns>PagedList<DistributorInfo></returns>
        public async Task<IPagedList<UserInformation>> GetUserInfos(IUnitOfWork unitOfWork)
        {
            //var unitOfWork = new UnitOfWork();

            var searchString = GetSearchString();
            var sortOrder = GetSortOrder();

            var list = await unitOfWork.UserInformations.GetUserListWithRolesAsync(filter: searchString, orderBy: sortOrder);

            //var pagedResult = await PagingList<BookListViewModel>.CreateAsync(query, 10, page);

            return list.ToPagedList(PageNumber, PageSize);
        }

        /// <summary>
        /// This method takes uses the searchstring property to create a lambda filter for the database query above
        /// </summary>
        /// <returns>lambda filter</returns>
        private Expression<Func<UserInformation, bool>> GetSearchString()
        {
            if (SearchString == null && CurrentFilter == null) return null;

            if (SearchString != null)
            {
                this.UserInformationId = null;
                Page = 1;
                CurrentFilter = SearchString;
                SearchString = null;
            }

            return (a => a.LastName.StartsWith(CurrentFilter) || a.FirstName.StartsWith(CurrentFilter));
        }

        /// <summary>
        /// This method uses the sortorder property to build a lambda order by expression for the database query above.
        /// </summary>
        /// <returns></returns>
        private Func<IQueryable<UserInformation>, IOrderedQueryable<UserInformation>> GetSortOrder()
        {
            switch (SortOrder)
            {
                case "Name_desc":
                    return q => q.OrderByDescending(d => d.LastName);

                case "FirstName_desc":
                    return q => q.OrderByDescending(d => d.FirstName);

                case "FirstName":
                    return q => q.OrderBy(d => d.FirstName);

                default:
                    return q => q.OrderBy(d => d.LastName);
            }
        }
    }
}