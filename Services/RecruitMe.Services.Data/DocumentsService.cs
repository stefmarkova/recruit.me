﻿namespace RecruitMe.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using RecruitMe.Data.Common.Repositories;
    using RecruitMe.Data.Models;
    using RecruitMe.Services;
    using RecruitMe.Services.Mapping;
    using RecruitMe.Web.ViewModels.Documents;

    public class DocumentsService : IDocumentsService
    {
        private readonly IFileDownloadService fileDownloadService;
        private readonly IDeletableEntityRepository<Document> documentRepository;
        private readonly Cloudinary cloudinary;

        public DocumentsService(
            IFileDownloadService fileDownloadService,
            IDeletableEntityRepository<Document> documentRepository,
            Cloudinary cloudinary)
        {
            this.fileDownloadService = fileDownloadService;
            this.documentRepository = documentRepository;
            this.cloudinary = cloudinary;
        }

        public IEnumerable<T> GetAllDocumentsForCandidate<T>(string candidateId)
        {
            return this.documentRepository
                .AllAsNoTracking()
                .Where(d => d.CandidateId == candidateId)
                .OrderBy(d => d.CreatedOn)
                .To<T>()
                .ToList();
        }

        public async Task<string> Upload(UploadInputModel model, string candidateId)
        {
            Document document = AutoMapperConfig.MapperInstance.Map<Document>(model);
            document.CandidateId = candidateId;
            string documentUrl = await CloudinaryService.UploadRawFileAsync(this.cloudinary, model.File, candidateId + $"_{document.Name}");
            if (documentUrl == null)
            {
                return null;
            }

            document.Url = documentUrl;
            document.CreatedOn = DateTime.UtcNow;
            try
            {
                await this.documentRepository.AddAsync(document);
                await this.documentRepository.SaveChangesAsync();
                return document.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DocumentNameAlreadyExists(string fileName)
        {
            return this.documentRepository
                 .AllAsNoTracking()
                 .Any(d => d.Name == fileName);
        }

        public async Task<bool> Delete(string documentId)
        {
            Document document = this.documentRepository
                .All()
                .FirstOrDefault(d => d.Id == documentId);

            CloudinaryService.DeleteFile(this.cloudinary, document.CandidateId + $"_{document.Name}");
            try
            {
                this.documentRepository.Delete(document);
                await this.documentRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsCandidateOwnerOfDocument(string candidateId, string documentId)
        {
            return this.documentRepository
                .AllAsNoTracking()
                .Any(d => d.Id == documentId
                  && d.CandidateId == candidateId);
        }

        public async Task<byte[]> Download(string documentId)
        {
            string documentUrl = this.documentRepository
                .AllAsNoTracking()
                .Where(d => d.Id == documentId)
                .Select(d => d.Url)
                .FirstOrDefault();

            byte[] file = await this.fileDownloadService.DownloadFile(documentUrl);

            return file;
        }

        public string GetDocumentNameById(string documentId)
        {
            return this.documentRepository
                  .AllAsNoTracking()
                  .Where(d => d.Id == documentId)
                  .Select(d => d.Name)
                  .FirstOrDefault();
        }
    }
}
