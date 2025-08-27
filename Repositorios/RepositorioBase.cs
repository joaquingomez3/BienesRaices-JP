using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bienesraices.Repositorios;

public abstract class RepositorioBase
	{
		protected readonly IConfiguration configuration;
		protected readonly string connectionString;

	protected RepositorioBase(IConfiguration configuration)
	{
		this.configuration = configuration;
		connectionString = configuration["ConnectionString:MySql"] ??
			throw new InvalidOperationException("Falta la cadena de conexión MySql en la configuración.");
		}
	}

