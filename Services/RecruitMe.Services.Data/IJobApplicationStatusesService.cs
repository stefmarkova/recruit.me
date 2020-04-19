﻿namespace RecruitMe.Services.Data
{
    using RecruitMe.Web.ViewModels.Administration.JobApplicationStatuses;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IJobApplicationStatusesService
    {
        IEnumerable<T> GetAll<T>();

        IEnumerable<T> GetAllWithDeleted<T>();

        Task<int> Create(CreateViewModel input);

        Task<int> Update(EditViewModel input);

        bool Delete(int id);

        T GetDetails<T>(int id);
    }
}
