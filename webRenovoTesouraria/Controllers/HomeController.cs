using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;
using webRenovoTesouraria.ViewModels;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace webRenovoTesouraria.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public IActionResult Index() {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("_header1"))) HttpContext.Session.SetString("_header1", "");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("_header2"))) HttpContext.Session.SetString("_header2", "");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("_hash"))) HttpContext.Session.SetString("_hash", "");
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormCollection formulario)
        {
            string nome1 = formulario["txNome1"];
            string nome2 = formulario["txNome2"];
            var retorno1 = Util.Funcoes.ValidarLogin(nome1);
            var retorno2 = Util.Funcoes.ValidarLogin(nome2);

            if (!String.IsNullOrEmpty(retorno1) || !String.IsNullOrEmpty(retorno2))
            {
                ViewBag.MensagemErro = "<b>Erros encontrados:<b> <br />" + retorno1 + "<br />" + retorno2;
                return View();
            }

            //INCLUIR O HEADER
            var hash_code = _unitOfWork.CabecalhosEntradas.Incluir(new ViewModels.CabecalhoEntradaVM { Nome1 = Util.DataCript.Encriptar(nome1.ToUpper()), Nome2 = Util.DataCript.Encriptar(nome2.ToUpper()) });

            //if (String.IsNullOrEmpty(hash_code))
            if (hash_code == 0)
            {
                ViewBag.MensagemErro = "Erros encontrados: " + retorno1 + "==> " + retorno2;
                return View();
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_header1"))) HttpContext.Session.SetString("_header1", Util.DataCript.Encriptar(nome1.ToUpper()));
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_header2"))) HttpContext.Session.SetString("_header2", Util.DataCript.Encriptar(nome2.ToUpper()));
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_hash"))) HttpContext.Session.SetString("_hash", Util.DataCript.Encriptar(hash_code.ToString()));

            return RedirectToAction("Entrada", "Home");
        }

        public IActionResult Entrada()
        {
            ViewBag.TiposEntrada = _unitOfWork.TiposEntrada.Listar();
            ViewBag.cboTipoEntrada = _unitOfWork.TiposEntrada.Listar()
                                        .Select(s => new SelectListItem() { Text = s.Nome, Value = s.ID.ToString() })
                                        .ToList();

            ViewBag.ConferidoPorUm = Util.DataCript.Decriptar(HttpContext.Session.GetString("_header1"));
            ViewBag.ConferidoPorDois = Util.DataCript.Decriptar(HttpContext.Session.GetString("_header2"));
            ViewBag.DiaSemana = Util.Funcoes.RetornaDia();
            ViewBag.Periodo = Util.Funcoes.RetornaPeriodo();

            //var value = GetInstanceField(typeof(Microsoft.AspNetCore.Session.DistributedSession), HttpContext.Session, "_idleTimeout");

            return View();
        }

        [HttpPost]
        public IActionResult AddEntrada([FromBody]EntradaRequest entradaRequest)
        {
            try
            {
                string erro = "";
                var vlTotalCategoria = entradaRequest.VlDizimo + entradaRequest.VlOferta + entradaRequest.VlMissoes + entradaRequest.VlReforma;
                var vlTotalTipo = entradaRequest.VlNotas + entradaRequest.VlMoedas + entradaRequest.VlCheques + entradaRequest.VlDebito + entradaRequest.VlCredito + entradaRequest.VlTransf;
                if (vlTotalCategoria != vlTotalTipo) throw new Exception("Os valores de Dizimos e Ofertas não conferem com o informado em Notas/Cartões/Transf. etc");

                //VALIDAR PESSOA
                int idPessoa;

                if (entradaRequest.Nome.ToUpper() != "ANÔNIMO")
                {
                    var pessoa = _unitOfWork.Pessoas.Selecionar(nomeCripto: Util.DataCript.Encriptar(entradaRequest.Nome.ToUpper()));

                    if (pessoa.Count > 0)
                        idPessoa = pessoa.FirstOrDefault().ID;
                    else
                        idPessoa = _unitOfWork.Pessoas.Incluir(Util.DataCript.Encriptar(entradaRequest.Nome.ToUpper()));
                }
                else
                {
                    idPessoa = 1;
                }

                //CATEGORIA
                if (entradaRequest.VlDizimo > 0) _unitOfWork.DetalhesCategoria.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 1, entradaRequest.VlDizimo);
                if (entradaRequest.VlOferta > 0) _unitOfWork.DetalhesCategoria.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 2, entradaRequest.VlOferta);
                if (entradaRequest.VlMissoes > 0) _unitOfWork.DetalhesCategoria.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 3, entradaRequest.VlMissoes);
                if (entradaRequest.VlReforma > 0) _unitOfWork.DetalhesCategoria.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 4, entradaRequest.VlReforma);

                //TIPO
                if (entradaRequest.VlNotas > 0) _unitOfWork.DetalhesTipo.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 1, entradaRequest.VlNotas);
                if (entradaRequest.VlMoedas > 0) _unitOfWork.DetalhesTipo.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 2, entradaRequest.VlMoedas);
                if (entradaRequest.VlCheques > 0) _unitOfWork.DetalhesTipo.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 3, entradaRequest.VlCheques);
                if (entradaRequest.VlDebito > 0) _unitOfWork.DetalhesTipo.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 4, entradaRequest.VlDebito);
                if (entradaRequest.VlCredito > 0) _unitOfWork.DetalhesTipo.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 5, entradaRequest.VlCredito);
                if (entradaRequest.VlTransf > 0) _unitOfWork.DetalhesTipo.Inserir(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), idPessoa, 6, entradaRequest.VlTransf);

                var listaDetalhes = _unitOfWork.CabecalhosEntradas.ListaDetalhes(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))));

                EntradaResponse total = new EntradaResponse
                {
                    VlTotalDizimo = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 1).Sum(s => s.Valor),
                    VlTotalOferta = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 2).Sum(s => s.Valor),
                    VlTotalMissoes = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 3).Sum(s => s.Valor),
                    VlTotalReforma = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 4).Sum(s => s.Valor),

                    VlTotalNotas = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 1).Sum(s => s.Valor),
                    VlTotalMoedas = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 2).Sum(s => s.Valor),
                    VlTotalCheque = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 3).Sum(s => s.Valor),
                    VlTotalDebito = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 4).Sum(s => s.Valor),
                    VlTotalCredito = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 5).Sum(s => s.Valor),
                    VlTotalTransf = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 6).Sum(s => s.Valor),
                };
                total.VlTotalCategoria = total.VlTotalDizimo + total.VlTotalOferta + total.VlTotalMissoes + total.VlTotalReforma;
                total.VlTotalTipo = total.VlTotalNotas + total.VlTotalMoedas + total.VlTotalCheque + total.VlTotalDebito + total.VlTotalCredito + total.VlTotalTransf;

                var lista = _unitOfWork.CabecalhosEntradas.ListarResponse(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))));

                RetornoResponse retorno = new RetornoResponse
                {
                    retornoTotais = total,
                    retornoLista = lista,
                    retornoErro = erro
                };

                return Json(retorno);
            }
            catch (Exception ex)
            {
                return BadRequest(Json(ex));
            }
        }

        [HttpPost]
        public IActionResult DelEntrada([FromBody]RemoveAPIRequest pessoaRequest)
        {
            try
            {
                var pessoa = _unitOfWork.Pessoas.Selecionar(nomeCripto: Util.DataCript.Encriptar(pessoaRequest.Nome.ToUpper())).FirstOrDefault();

                var remover = _unitOfWork.CabecalhosEntradas.ExcluirEntrada(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))), pessoa.ID);
                var listaDetalhes = _unitOfWork.CabecalhosEntradas.ListaDetalhes(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))));

                EntradaResponse total = new EntradaResponse
                {
                    VlTotalDizimo = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 1).Sum(s => s.Valor),
                    VlTotalOferta = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 2).Sum(s => s.Valor),
                    VlTotalMissoes = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 3).Sum(s => s.Valor),
                    VlTotalReforma = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 4).Sum(s => s.Valor),

                    VlTotalNotas = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 1).Sum(s => s.Valor),
                    VlTotalMoedas = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 2).Sum(s => s.Valor),
                    VlTotalCheque = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 3).Sum(s => s.Valor),
                    VlTotalDebito = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 4).Sum(s => s.Valor),
                    VlTotalCredito = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 5).Sum(s => s.Valor),
                    VlTotalTransf = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 6).Sum(s => s.Valor),
                };
                total.VlTotalCategoria = total.VlTotalDizimo + total.VlTotalOferta + total.VlTotalMissoes + total.VlTotalReforma;
                total.VlTotalTipo = total.VlTotalNotas + total.VlTotalMoedas + total.VlTotalCheque + total.VlTotalDebito + total.VlTotalCredito + total.VlTotalTransf;

                var lista = _unitOfWork.CabecalhosEntradas.ListarResponse(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))));

                RetornoResponse retorno = new RetornoResponse
                {
                    retornoErro = "",
                    retornoTotais = total,
                    retornoLista = lista
                };
                return Json(retorno);
            }
            catch (Exception ex)
            {
                return Json(BadRequest(ex));
            }
        }

        public IActionResult Finalizar()
        {
            try
            {
                Document relatorio = CriaRelatorio();
                relatorio.UseCmykColor = true;
                PdfDocumentRenderer pdfRenderCategoria = new PdfDocumentRenderer(false);
                pdfRenderCategoria.Document = relatorio;
                pdfRenderCategoria.RenderDocument();
                string nomeArquivoCategoria = "RelatorioGeral_" + System.DateTime.Now.ToString("yyyyMMddhhmm") + ".pdf";
                pdfRenderCategoria.PdfDocument.Save(nomeArquivoCategoria);

                //Response.Headers.Add("Content-Disposition", $"inline; filename='RelatorioGeral.pdf'");
                //return File("RelatorioGeral.pdf", "application/pdf");
                //return View();
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                pdfRenderCategoria.PdfDocument.Save(stream, false);                
                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Document CriaRelatorio()
        {
            Document relatorio = new Document();
            Section section = relatorio.AddSection();

            MigraDoc.DocumentObjectModel.Shapes.Image img = section.Headers.Primary.AddImage(@"C:\Users\gperez\source\repos\RenovoTesouraria\RenovoTesouraria\Content\gustavo2.png");
            img.Height = "15cm";
            img.LockAspectRatio = true;
            img.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;
            img.RelativeHorizontal = MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal.Page;
            img.Top = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Center;
            img.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Center;
            img.WrapFormat.Style = MigraDoc.DocumentObjectModel.Shapes.WrapStyle.Through;

            section.PageSetup.TopMargin = 25;
            section.PageSetup.LeftMargin = 25;
            section.PageSetup.RightMargin = 25;
            section.PageSetup.BottomMargin = 25;
            section.PageSetup.PageFormat = PageFormat.A4;

            #region HeaderRelatorio
            Paragraph primeira_linha = section.AddParagraph("RELATÓRIO DE ENTRADA DÍZIMOS E OFERTAS - IGREJA BATISTA RENOVO");
            primeira_linha.Format.Alignment = ParagraphAlignment.Center;

            MigraDoc.DocumentObjectModel.Tables.Table tabela_header = new MigraDoc.DocumentObjectModel.Tables.Table();
            MigraDoc.DocumentObjectModel.Tables.Row linha_header;
            tabela_header.Borders.Width = 0.5;

            MigraDoc.DocumentObjectModel.Tables.Column coluna_header = tabela_header.AddColumn("18cm");
            coluna_header.Format.Font.Name = "Calibri";
            coluna_header.Shading.Color = Colors.WhiteSmoke;
            linha_header = tabela_header.AddRow();
            linha_header.Format.Alignment = ParagraphAlignment.Center;
            linha_header.Format.Font.Bold = true;
            linha_header.Cells[0].AddParagraph(System.DateTime.Now.ToString("dd/MM/yyyy") + " - " + Util.Funcoes.RetornaDia() + "(" + Util.Funcoes.RetornaPeriodo() + ")");
            tabela_header.SetEdge(0, 0, 0, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);
            relatorio.LastSection.Add(tabela_header);
            #endregion
            Paragraph quebra_linha_headerReport = section.AddParagraph("");

            #region HeaderTabela
            MigraDoc.DocumentObjectModel.Tables.Table tabela_dizimistas;
            MigraDoc.DocumentObjectModel.Tables.Row linha_dizimistas;

            tabela_dizimistas = new MigraDoc.DocumentObjectModel.Tables.Table();
            tabela_dizimistas.Borders.Width = 0.5;

            MigraDoc.DocumentObjectModel.Tables.Column coluna1 = tabela_dizimistas.AddColumn("14cm");
            coluna1.Format.Font.Name = "Calibri";

            MigraDoc.DocumentObjectModel.Tables.Column coluna2 = tabela_dizimistas.AddColumn("4cm");
            coluna2.Format.Font.Name = "Calibri";

            linha_dizimistas = tabela_dizimistas.AddRow();
            linha_dizimistas.Shading.Color = Colors.Black;
            linha_dizimistas.Format.Font.Color = Colors.White;
            linha_dizimistas.Format.Font.Bold = true;
            linha_dizimistas.Format.Alignment = ParagraphAlignment.Center;
            linha_dizimistas.Cells[0].AddParagraph("DIZIMISTA");
            linha_dizimistas.Cells[1].AddParagraph("VALOR");
            #endregion

            #region ItensTabela
            decimal total_dizimos = 0;

            var listaDetalhes = _unitOfWork.CabecalhosEntradas.ListaDetalhes(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))));

            EntradaResponse total = new EntradaResponse
            {
                VlTotalDizimo = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 1).Sum(s => s.Valor),
                VlTotalOferta = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 2).Sum(s => s.Valor),
                VlTotalMissoes = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 3).Sum(s => s.Valor),
                VlTotalReforma = listaDetalhes[0].ItensCategoria.Where(w => w.IdCategoria == 4).Sum(s => s.Valor),

                VlTotalNotas = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 1).Sum(s => s.Valor),
                VlTotalMoedas = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 2).Sum(s => s.Valor),
                VlTotalCheque = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 3).Sum(s => s.Valor),
                VlTotalDebito = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 4).Sum(s => s.Valor),
                VlTotalCredito = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 5).Sum(s => s.Valor),
                VlTotalTransf = listaDetalhes[0].ItensTipo.Where(w => w.IdTipo == 6).Sum(s => s.Valor),
            };
            total.VlTotalCategoria = total.VlTotalDizimo + total.VlTotalOferta + total.VlTotalMissoes + total.VlTotalReforma;
            total.VlTotalTipo = total.VlTotalNotas + total.VlTotalMoedas + total.VlTotalCheque + total.VlTotalDebito + total.VlTotalCredito + total.VlTotalTransf;

            var lista = _unitOfWork.CabecalhosEntradas.ListarResponse(Convert.ToInt32(Util.DataCript.Decriptar(HttpContext.Session.GetString("_hash"))));

            RetornoResponse retorno = new RetornoResponse
            {
                retornoTotais = total,
                retornoLista = lista
            };

            foreach (var linha in retorno.retornoLista.Where(w => w.Dizimo > 0))
            {
                linha_dizimistas = tabela_dizimistas.AddRow();
                linha_dizimistas.Format.Font.Size = 9;
                //linha_dizimistas.Cells[0].AddParagraph(Util.DataCript.Decriptar(linha.Pessoa.Nome).ToUpper());
                linha_dizimistas.Cells[0].AddParagraph(linha.Pessoa.Nome.ToUpper());
                linha_dizimistas.Cells[1].AddParagraph(linha.Dizimo.ToString());
                total_dizimos += linha.Dizimo;
            }

            tabela_dizimistas.SetEdge(0, 0, 0, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);
            relatorio.LastSection.Add(tabela_dizimistas);
            #endregion

            Paragraph quebra_linha_totalizadores = section.AddParagraph("");
            Paragraph quebra_linha_totalizadores2 = section.AddParagraph("");
            #region Totalizadores
            decimal total_ofertas = 0;
            decimal total_missoes = 0;
            decimal total_infantil = 0;
            decimal total_reforma = 0;

            total_ofertas = retorno.retornoTotais.VlTotalOferta;
            total_missoes = retorno.retornoTotais.VlTotalMissoes;
            total_infantil = 0;
            total_reforma = retorno.retornoTotais.VlTotalReforma;

            MigraDoc.DocumentObjectModel.Tables.Table tabela_total;
            MigraDoc.DocumentObjectModel.Tables.Row linha_total;

            tabela_total = new MigraDoc.DocumentObjectModel.Tables.Table();
            tabela_total.Borders.Width = 0.5;
            MigraDoc.DocumentObjectModel.Tables.Column coluna_total1 = tabela_total.AddColumn("14cm");
            coluna_total1.Format.Font.Name = "Calibri";

            MigraDoc.DocumentObjectModel.Tables.Column coluna_total2 = tabela_total.AddColumn("4cm");
            coluna_total2.Format.Font.Name = "Calibri";

            linha_total = tabela_total.AddRow();
            linha_total.Shading.Color = Colors.Gray;
            linha_total.Format.Alignment = ParagraphAlignment.Center;
            linha_total.Cells[0].AddParagraph("TOTAL DIZIMOS");
            linha_total.Cells[1].AddParagraph(total_dizimos.ToString());

            linha_total = tabela_total.AddRow();
            linha_total.Shading.Color = Colors.Gray;
            linha_total.Format.Alignment = ParagraphAlignment.Center;
            linha_total.Cells[0].AddParagraph("TOTAL OFERTAS");
            linha_total.Cells[1].AddParagraph(total_ofertas.ToString());

            linha_total = tabela_total.AddRow();
            linha_total.Shading.Color = Colors.Gray;
            linha_total.Format.Alignment = ParagraphAlignment.Center;
            linha_total.Cells[0].AddParagraph("TOTAL MISSÕES");
            linha_total.Cells[1].AddParagraph(total_missoes.ToString());

            linha_total = tabela_total.AddRow();
            linha_total.Shading.Color = Colors.Gray;
            linha_total.Format.Alignment = ParagraphAlignment.Center;
            linha_total.Cells[0].AddParagraph("TOTAL REFORMA");
            linha_total.Cells[1].AddParagraph(total_reforma.ToString());

            linha_total = tabela_total.AddRow();
            linha_total.Shading.Color = Colors.Gray;
            linha_total.Format.Alignment = ParagraphAlignment.Center;
            linha_total.Cells[0].AddParagraph("TOTAL INFANTIL");
            linha_total.Cells[1].AddParagraph(total_infantil.ToString());

            linha_total = tabela_total.AddRow();
            linha_total.Shading.Color = Colors.DarkGray;
            linha_total.Format.Font.Bold = true;
            linha_total.Format.Alignment = ParagraphAlignment.Center;
            linha_total.Cells[0].AddParagraph("TOTAL");
            var totalCateg = total_dizimos + total_ofertas + total_missoes + total_reforma + total_infantil;
            linha_total.Cells[1].AddParagraph(totalCateg.ToString());

            tabela_total.SetEdge(0, 0, 0, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);
            relatorio.LastSection.Add(tabela_total);
            #endregion

            Paragraph quebra_linha_tipo = section.AddParagraph("");
            Paragraph quebra_linha_tipo2 = section.AddParagraph("");
            #region RelatorioTipo
            MigraDoc.DocumentObjectModel.Tables.Table tabela_tipo;
            MigraDoc.DocumentObjectModel.Tables.Row linha_tipo;

            tabela_tipo = new MigraDoc.DocumentObjectModel.Tables.Table();
            tabela_tipo.Borders.Width = 0.5;
            MigraDoc.DocumentObjectModel.Tables.Column coluna_nota = tabela_tipo.AddColumn("3cm");
            MigraDoc.DocumentObjectModel.Tables.Column coluna_moeda = tabela_tipo.AddColumn("3cm");
            MigraDoc.DocumentObjectModel.Tables.Column coluna_cheque = tabela_tipo.AddColumn("3cm");
            MigraDoc.DocumentObjectModel.Tables.Column coluna_debito = tabela_tipo.AddColumn("3cm");
            MigraDoc.DocumentObjectModel.Tables.Column coluna_credito = tabela_tipo.AddColumn("3cm");
            MigraDoc.DocumentObjectModel.Tables.Column coluna_transf = tabela_tipo.AddColumn("3cm");

            coluna_nota.Format.Font.Name = "Calibri";
            coluna_moeda.Format.Font.Name = "Calibri";
            coluna_cheque.Format.Font.Name = "Calibri";
            coluna_debito.Format.Font.Name = "Calibri";
            coluna_credito.Format.Font.Name = "Calibri";
            coluna_transf.Format.Font.Name = "Calibri";

            linha_tipo = tabela_tipo.AddRow();
            linha_tipo.Shading.Color = Colors.Black;
            linha_tipo.Format.Font.Color = Colors.White;
            linha_tipo.Format.Font.Bold = true;
            linha_tipo.Cells[0].AddParagraph("Notas");
            linha_tipo.Cells[1].AddParagraph("Moedas");
            linha_tipo.Cells[2].AddParagraph("Cheques");
            linha_tipo.Cells[3].AddParagraph("C. Déb.");
            linha_tipo.Cells[4].AddParagraph("C. Créd.");
            linha_tipo.Cells[5].AddParagraph("Transf.");

            decimal total_notas = 0;
            decimal total_moedas = 0;
            decimal total_cheques = 0;
            decimal total_debito = 0;
            decimal total_credito = 0;
            decimal total_transf = 0;

            total_notas = retorno.retornoTotais.VlTotalNotas;
            total_moedas = retorno.retornoTotais.VlTotalMoedas;
            total_cheques = retorno.retornoTotais.VlTotalCheque;
            total_debito = retorno.retornoTotais.VlTotalDebito;
            total_credito = retorno.retornoTotais.VlTotalCredito;
            total_transf = retorno.retornoTotais.VlTotalTransf;

            linha_tipo = tabela_tipo.AddRow();
            linha_tipo.Cells[0].AddParagraph(total_notas.ToString());
            linha_tipo.Cells[1].AddParagraph(total_moedas.ToString());
            linha_tipo.Cells[2].AddParagraph(total_cheques.ToString());
            linha_tipo.Cells[3].AddParagraph(total_debito.ToString());
            linha_tipo.Cells[4].AddParagraph(total_credito.ToString());
            linha_tipo.Cells[5].AddParagraph(total_transf.ToString());
            Paragraph quebra_linha5 = section.AddParagraph("");

            tabela_tipo.SetEdge(0, 0, 0, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);
            relatorio.LastSection.Add(tabela_tipo);

            #endregion

            Paragraph quebra_linha_conferidoPor = section.AddParagraph("");
            Paragraph quebra_linha_conferidoPor2 = section.AddParagraph("");
            Paragraph quebra_linha_conferidoPor3 = section.AddParagraph("");
            #region ConferidoPor
            MigraDoc.DocumentObjectModel.Tables.Table tabela_conferidoPor;
            MigraDoc.DocumentObjectModel.Tables.Row linha_conferidoPor;

            tabela_conferidoPor = new MigraDoc.DocumentObjectModel.Tables.Table();
            tabela_conferidoPor.Borders.Width = 0;
            MigraDoc.DocumentObjectModel.Tables.Column coluna_conferidoPor = tabela_conferidoPor.AddColumn("3cm");
            coluna_conferidoPor.Format.Font.Name = "Calibri";

            MigraDoc.DocumentObjectModel.Tables.Column coluna_conferidoPor1 = tabela_conferidoPor.AddColumn("7cm");
            coluna_conferidoPor1.Format.Font.Name = "Calibri";

            MigraDoc.DocumentObjectModel.Tables.Column coluna_conferidoPor2 = tabela_conferidoPor.AddColumn("7cm");
            coluna_conferidoPor2.Format.Font.Name = "Calibri";

            linha_conferidoPor = tabela_conferidoPor.AddRow();
            linha_conferidoPor.Format.Alignment = ParagraphAlignment.Center;
            linha_conferidoPor.Cells[0].AddParagraph("CONFERIDO POR:");
            linha_conferidoPor.Cells[1].AddParagraph(Util.DataCript.Decriptar(HttpContext.Session.GetString("_header1")));
            linha_conferidoPor.Cells[1].Format.Borders.Top.Width = 0.5;
            linha_conferidoPor.Cells[2].AddParagraph(Util.DataCript.Decriptar(HttpContext.Session.GetString("_header2")));
            linha_conferidoPor.Cells[2].Format.Borders.Top.Width = 0.5;

            tabela_conferidoPor.SetEdge(0, 0, 0, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);
            relatorio.LastSection.Add(tabela_conferidoPor);
            #endregion   

            return relatorio;
        }
        
    }

    struct RetornoResponse
    {
        public EntradaResponse retornoTotais { get; set; }
        public List<ListaVM> retornoLista { get; set; }
        public string retornoErro { get; set; }
    }
}