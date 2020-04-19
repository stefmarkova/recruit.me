﻿namespace RecruitMe.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RecruitMe.Web.ViewModels.Documents;

    public interface IDocumentsService
    {
        IEnumerable<T> GetAllDocumentsForCandidate<T>(string candidateId);

        string GetDocumentNameById(string documentId);

        Task<string> Upload(UploadInputModel model, string candidateId);

        bool DocumentNameAlreadyExists(string fileName);

        Task<bool> Delete(string documentId);

        Task<byte[]> Download(string documentId);

        bool IsCandidateOwnerOfDocument(string candidateId, string documentId);
    }
}
