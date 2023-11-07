using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.Constants;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Pipes;
using RecruitmentManagementAPI.Controllers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace RecruitmentManagementAPI.Services
{
    public class CommonUtils
    {
        private readonly APISettings _apiSettings;
        private readonly ILogger<CommonUtils> _logger;
        public CommonUtils(ILogger<CommonUtils> logger, IOptions<APISettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
            _logger = logger;
        }

        public ActionResult GetResult(ControllerBase controller, APIResponse response)
        {
            return response.Result == null && response.ErrorMessages.Count == 0 ? controller.StatusCode((int)response.StatusCode) : controller.StatusCode((int)response.StatusCode, response);
        }

        public bool IsStrongPassword(string password)
        {
            // Regular expression pattern to enforce password requirements
            // Default: Atleast 1 digit, 1 simbol, 1 uppercase, 1 lowercase and minimum length 8
            return Regex.IsMatch(password, _apiSettings.PasswordRequirementsPattern);
        }

        public string JoinStringList(List<string> list, string delimiter = ", ")
        {
            return string.Join(delimiter, list);
        }
        public bool CheckCsvContentIsCorrect(List<List<string>> lines)
        {
            return AllListAreTheSameLength(lines);
        }

        public bool AllListAreTheSameLength(List<List<string>> lists)
        {
            return lists.All(list => list.Count == lists.First().Count);;
        }

        public bool CreateCSVFile(string fileName, List<List<string>> csvContent, out string filePath)
        {
            bool isCreated = false;
            filePath = string.Empty;

            try
            {
                CreateCsvFolderIfNotExists();
                filePath = Path.Combine(GetCsvFolderPath(), fileName);

                if (!File.Exists(filePath))
                {
                    WriteCSVFileContent(csvContent, filePath);
                    isCreated = true;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }

            return isCreated;
        }

        public void CreateCsvFolderIfNotExists()
        {
            string csvFolderPath = GetCsvFolderPath();
            if (!Directory.Exists(csvFolderPath))
            {
                Directory.CreateDirectory(csvFolderPath);
            }
        }
        public string GetCsvFolderPath()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(currentPath, _apiSettings.CsvLocalStorageFolderName);
        }

        public bool DeleteCSVFile(string filePath)
        {
            bool isDeleted = false;

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                isDeleted = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }

            return isDeleted;
        }

        public string RenameCSVFile(string fileToRenamePath, string newFileName )
        {
            string newPath = null;
            try
            {                
                if (File.Exists(fileToRenamePath))
                {
                    newPath = Path.Combine(Path.GetDirectoryName(fileToRenamePath), newFileName);
                    File.Move(fileToRenamePath, newPath);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message.ToString());
            }

            return newPath;
        }

        public void WriteCSVFileContent(List<List<string>> csvContent, string filePath, bool apend = true)
        {
            using (StreamWriter writer = new StreamWriter(filePath, apend))
            {
                foreach (List<string> csvLine in csvContent)
                {
                    writer.WriteLine(this.JoinStringList(csvLine));
                }
            }
        }

        public bool UploadCSVFile(string uploadFilePath, out string tarjetPath, bool overwrite = true)
        {
            bool isCopied = false;
            tarjetPath = Path.Combine(GetCsvFolderPath(), Path.GetFileName(uploadFilePath));

            try
            {
                if (File.Exists(uploadFilePath))
                {
                    
                    File.Copy(uploadFilePath, tarjetPath, overwrite);
                    isCopied = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }

            return isCopied;
        }

        public string AddFileExtension(string fileName, string extension)
        { 
            return $"{fileName}.{extension}";
        }
    }
}
