using APICatalogoJogos.Exceptions;
using APICatalogoJogos.InputModel;
using APICatalogoJogos.Services;
using APICatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {

        private readonly IJogoService _jogoService;

        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;

        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possível retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página está sendo consultada, Mínimo 1</param>
        /// <param name="quantidade">Indica a quantidade de registros por página. Mínimo 1 e mácimo 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>
        [HttpGet]
        public async Task<ActionResult<List<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count() == 0)
                return NoContent();

            return Ok(jogos);
        }

        /// <summary>
        /// Buscar um jogo pelo seu id
        /// </summary>
        /// <param name="idJogo">Id do Jogo buscado</param>
        /// <response code="200">Retorna o jogo filtrado</response>
        /// <response code="204">Caso não haja jogo com este id</response>
        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();

            return Ok(jogo);
        }

        /// <summary>
        /// Insere um novo jogo
        /// </summary>
        /// <param name="jogoInputModel">Model contendo os dados do jogo a ser cadastrado</param>
        /// <response code="200">Retorna o jogo cadastrado</response>
        /// <response code="422">Caso já existe jogo com estes dados</response>
        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);

                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)            
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora");
            }
        }


        /// <summary>
        /// Atualiza dados de um jogo
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="jogoInputModel">Model contendo os dados do jogo a ser cadastrado</param>
        /// <response code="200">Retorna sucesso</response>
        /// <response code="404">Caso o jogo não exista</response>
        [HttpPut("{idJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);

                return Ok();
            }
            catch (JogoJaCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Atualiza o preço do jogo
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="preco">Novo preço</param>
        /// <response code="200">Retorna sucesso</response>
        /// <response code="404">Caso o jogo não exista</response>
        [HttpPatch("{idJogo:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);

                return Ok();
            }
            catch (JogoJaCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }


        /// <summary>
        /// Apaga o jogo solicitado
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser excluído</param>
        /// <response code="200">Retorna sucesso</response>
        /// <response code="404">Caso o jogo não exista</response>
        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);

                return Ok();
            }
            catch (JogoJaCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

    }
}
