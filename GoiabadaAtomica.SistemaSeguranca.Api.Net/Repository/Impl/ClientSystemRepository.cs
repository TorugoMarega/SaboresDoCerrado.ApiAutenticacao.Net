using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class ClientSystemRepository : IClientSystemRepository
    {
        private readonly ApplicationContext _applicationContext;
        private readonly Logger<ClientSystemRepository> _logger;

        public ClientSystemRepository(ApplicationContext applicationContext, Logger<ClientSystemRepository> logger)
        {
            _applicationContext = applicationContext;
            _logger = logger;
        }

        public async Task<ClientSystemEntity?> GetClientSystemEntityByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca de Sistema do Cliente pelo id [{id}]", id);

            var entity = await _applicationContext.ClientSystemEntity
                .FirstOrDefaultAsync(clientSystem => clientSystem.Id == id);

            _logger.LogDebug("Busca de Sistema do Cliente finalizada com sucesso!");
            return entity;
        }
        public async Task<ClientSystemDTO?> GetClientSystemDTOByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca de Sistema do Cliente pelo id [{id}]", id);

            var clientSystemDTO = await _applicationContext.ClientSystemEntity
                .AsNoTracking()
                .Select(clientSystem => new ClientSystemDTO
                {
                    Id = clientSystem.Id,
                    Name = clientSystem.Name,
                    Description = clientSystem.Description,
                    ClientId = clientSystem.ClientId,
                    IsActive = clientSystem.IsActive
                })
                .FirstOrDefaultAsync(clientSystem => clientSystem.Id == id);
            _logger.LogDebug("Busca de Sistema do Cliente finalizada com sucesso!");
            return clientSystemDTO;
        }
        public async Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemByAsync()
        {
            _logger.LogInformation("Iniciando busca de todos os Sistemas");
            var clientSystemDTOList = await _applicationContext.ClientSystemEntity
                .AsNoTracking()
                .Select(clientSystem => new ClientSystemDTO
                {
                    Id = clientSystem.Id,
                    Name = clientSystem.Name,
                    Description = clientSystem.Description,
                    ClientId = clientSystem.ClientId,
                    IsActive = clientSystem.IsActive
                })
                .ToListAsync();
            _logger.LogDebug("Busca de Sistemas finalizada com sucesso!");
            return clientSystemDTOList;
        }
        public async Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(ClientSystemEntity clientSystem)
        {
            _logger.LogInformation("Persistindo sistema no banco de dados");
            await _applicationContext.ClientSystemEntity.AddAsync(clientSystem);
            await _applicationContext.SaveChangesAsync();
            return clientSystem.Adapt<CreateClientSystemResponseDTO>();
        }
        public async Task<int> UpdateClientSystemAsync(ClientSystemEntity clientSystem)
        {
            _logger.LogInformation("Atualizando sistema no banco de dados");
            _applicationContext.ClientSystemEntity.Update(clientSystem);
            return await _applicationContext.SaveChangesAsync();
        }
        public async Task<bool> ExistsClientSystemById(int id)
        {
            _logger.LogInformation("Validando existencia do sistema no banco de dados");
            var exists = await _applicationContext.ClientSystemEntity
                .AsNoTracking()
                .AnyAsync(clientSystem => clientSystem.Id.Equals(id));
            return exists;
        }
        public async Task<bool> ExistsClientSystemByNameAsync(string name)
        {
            _logger.LogInformation("Validando existencia do sistema no banco de dados");
            var exists = await _applicationContext.ClientSystemEntity
                .AsNoTracking()
                .AnyAsync(clientSystem => clientSystem.Name.ToLower().Equals(name.ToLower()));
            return exists;
        }
        public async Task<bool> ExistsClientSystemByClientIdAsync(string clientId)
        {
            _logger.LogInformation("Validando existencia do sistema no banco de dados");
            var exists = await _applicationContext.ClientSystemEntity
                .AsNoTracking()
                .AnyAsync(clientSystem => clientSystem.ClientId.Equals(clientId));
            return exists;
        }
    }
}
