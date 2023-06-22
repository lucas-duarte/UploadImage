using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UploadImage.API.Interfaces;
using UploadImage.API.Services;

namespace UploadImage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly IImageService _ImageService;

        public ImageController(BlobStorageService blobStorageService, IImageService imageService)
        {
            _blobStorageService = blobStorageService;
            _ImageService = imageService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum arquivo foi enviado.");

            var newGuid = Guid.NewGuid();

            var result = await _blobStorageService.CreateImageAsync(file, newGuid);

            await _ImageService.SaveImageAsync(result.ImagePath, newGuid, result.ExtensionImage);

            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var result = await _ImageService.GetAsync(id);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return NotFound(); // A imagem não foi encontrada
            }

            // Delete a imagem do Blob Storage
            var success = await _blobStorageService.DeleteFileAsync($"{result.Result.Id}{result.Result.ExtensionImage}");

            if (!success)
            {
                return StatusCode(500); // Ocorreu um erro ao excluir a imagem do Blob Storage
            }
           
            // Remova a imagem do banco de dados
            var response = await _ImageService.DeleteImageAsync(id);

            if (!response.Result)
            {
                return NotFound(); // A imagem não foi encontrada
            }

            return Ok(); // A imagem foi excluída com sucesso
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(Guid id, IFormFile file)
        {
            var result = await _ImageService.GetAsync(id);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return NotFound(); // A imagem não foi encontrada
            }

            // Delete a imagem existente do Blob Storage
            var success = await _blobStorageService.DeleteFileAsync($"{result.Result.Id}{result.Result.ExtensionImage}");

            if (!success)
            {
                return StatusCode(500); // Ocorreu um erro ao excluir a imagem do Blob Storage
            }

            // Crie uma nova imagem no Blob Storage
            var dataImageBlob = await _blobStorageService.CreateImageAsync(file, result.Result.Id);

            // Atualize a imagem no banco de dados
            result.Result.ExtensionImage = Path.GetExtension(file.FileName); // Atualize com a nova extensão do arquivo, se necessário
            result.Result.ImagePath = dataImageBlob.ImagePath;

            var updateResult = await _ImageService.UpdateAsync(result.Result);

            if (!updateResult.Result)
            {
                return StatusCode(500); // Ocorreu um erro ao atualizar a imagem no banco de dados
            }

            return Ok(); // A imagem foi atualizada com sucesso
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetImages()
        {
            var result = await _ImageService.GetLisImagens(); // Obtenha a lista de imagens do serviço

            if (String.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result); // Retorne a lista de imagens no corpo da resposta HTTP 200 OK
            }
            else
            {
                return BadRequest(result.ErrorMessage); // Retorne a mensagem de erro no corpo da resposta HTTP 400 Bad Request
            }
        }
    }
}
