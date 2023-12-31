﻿using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using System;

namespace HelpDeskTicketing.Data.Services
{
    public class TicketFilesService : ITicketFilesService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TicketFilesService(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> AddFileToDatabase(IEnumerable<IFormFile> files, string ticketId)
        {

            bool isFileSaved = false;

            if (!Directory.Exists(_webHostEnvironment.WebRootPath + "/Files"))
            {

                Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "/Files");

            }

            //Add Files to Database and Save to disk

            foreach (var file in files)
            {

                //Create an encoded name for the saved file
                string encodedFileName = string.Concat(Guid.NewGuid().ToString(), file.FileName.AsSpan(file.FileName.IndexOf("."))); 

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath + "/Files", encodedFileName);

                using (FileStream fileStream = File.Create(filePath))
                {

                    await file.CopyToAsync(fileStream);
                    isFileSaved= true;
                    
                }

                //Add file to database
                TicketFile ticketFile = new TicketFile
                {
                    Id = Guid.NewGuid().ToString(),
                    TicketMessageId = ticketId,
                    FileName = encodedFileName,
                    DisplayName = file.FileName
                };


                await _context.TicketFiles.AddAsync(ticketFile);
            
            }

            if (await _context.SaveChangesAsync() < 1)
            {

                return false;

            }

            return isFileSaved;

        }

        public Task<IEnumerable<IFormFile>> GetFilesFormDatabase(string ticketId)
        {
            throw new NotImplementedException();
        }
    }
}
