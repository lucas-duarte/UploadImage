using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using UploadImage.Web.Helpers;
using UploadImage.Web.Models;
using UploadImage.Web.Shared;

namespace UploadImage.Web.Pages
{
    public partial class Index : ComponentBase
    {
        private IBrowserFile? File { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }


        private bool Busy { get; set; } = true;

        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";

        private string DragClass = DefaultDragClass;

        public List<Image> Images { get; set; } = new List<Image>();

        [Inject]
        private ISnackbar Snackbar { get; set; }


        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                GetImagens();

            base.OnAfterRender(firstRender);
        }

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            if (e.File.Size > 512000)
            {
                Snackbar.Add("Não é possível enviar imagens maior que 512000 bytes", Severity.Error);
                return;
            }

            ClearDragClass();
            File = e.File;

            /*
            var arquivo = e.File;
            var stream = arquivo.OpenReadStream();
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                ImagemData = memoryStream.ToArray();
            }*/
        }

        private async Task Clear()
        {
            File = null;
            ClearDragClass();
            await Task.Delay(100);
        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }

        private async Task UploadImage()
        {
            Busy = true;

            if (File != null)
            {
                var content = new MultipartFormDataContent();

                content.Add(new StreamContent(File.OpenReadStream()), "file", File.Name);

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync("https://upload-imagens-api.azurewebsites.net/api/Image/Create", content);

                    Busy = false;

                    if (response.IsSuccessStatusCode)
                    {
                        // Imagem enviada com sucesso
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                        Snackbar.Add("Imagem enviada com sucesso!", Severity.Success);
                        await GetImagens();
                        await Clear();
                    }
                    else
                    {
                        // Ocorreu um erro ao enviar a imagem
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                        Snackbar.Add("Erro ao enviar a imagem. Status Code: " + response.StatusCode, Severity.Error);
                    }

                    StateHasChanged();
                }
            }
        }

        private async Task GetImagens()
        {
            Busy = true;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://upload-imagens-api.azurewebsites.net/api/Image/List");

                Busy = false;

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ProviderResult<List<Image>>>(json);

                    if (String.IsNullOrEmpty(result?.ErrorMessage))
                    {
                        Images = result?.Result;
                    }
                    else
                    {
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                        Snackbar.Add("Erro ao obter as imagens. Error: " + result.ErrorMessage);
                    }
                }
                else
                {
                    // Ocorreu um erro ao obter as imagens
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Erro ao obter as imagens. Status Code: " + response.StatusCode, Severity.Error);
                }

                StateHasChanged();
            }
        }
        private async Task DeleteImage(Guid id)
        {
            Busy = true;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync($"https://upload-imagens-api.azurewebsites.net/api/Image/{id}");

                Busy = false;

                if (response.IsSuccessStatusCode)
                {
                    // Imagem excluída com sucesso
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Imagem excluída com sucesso!", Severity.Success);
                    await GetImagens();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Imagem não encontrada
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Imagem não encontrada.", Severity.Error);
                }
                else
                {
                    // Ocorreu um erro ao excluir a imagem
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Erro ao excluir a imagem. Status Code: " + response.StatusCode, Severity.Error);
                }

                StateHasChanged();
            }
        }
        private async Task UpdateImage(Guid id)
        {
            Busy = true;

            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(File.OpenReadStream()), "file", File.Name);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsync($"https://upload-imagens-api.azurewebsites.net/api/Image/{id}", content);

                Busy = false;

                if (response.IsSuccessStatusCode)
                {
                    // Imagem atualizada com sucesso
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Imagem atualizada com sucesso!", Severity.Success);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Imagem não encontrada
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Imagem não encontrada.", Severity.Error);
                }
                else
                {
                    // Ocorreu um erro ao atualizar a imagem
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add("Erro ao atualizar a imagem. Status Code: " + response.StatusCode, Severity.Error);
                }

                await Clear();

                NavigationManager.NavigateTo("/", forceLoad: true);

            }
        }


        private async void OpenAlertDeleteImage(Guid id)
        {
            bool state;

            var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };

            bool? result = await DialogService.ShowMessageBox(
                "Aviso",
                "Deseja deletar está imagem?",
                yesText: "Deletar", noText: null, cancelText: "Cancelar", options);
            state = result == null ? false : true;

            if (state)
            {
                await DeleteImage(id);
            }

            StateHasChanged();
        }

        private async void OpenAlertUpdateImage(Guid id, InputFileChangeEventArgs e)
        {


            if (e.File.Size > 512000)
            {
                Snackbar.Add("Não é possível enviar imagens maior que 512000 bytes", Severity.Error);
                return;
            }
            
            File = e.File;

            bool state;

            var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };

            bool? result = await DialogService.ShowMessageBox(
                "Aviso",
                "Deseja atualizar está imagem?",
                yesText: "Atualizar", noText: null, cancelText: "Cancelar", options);
            state = result == null ? false : true;

            if (state)
            {
                var files = new List<IBrowserFile>();
                await UpdateImage(id);
            }
            else
            {
                await Clear();
            }

            StateHasChanged();

            await GetImagens();
        }
    }


}
