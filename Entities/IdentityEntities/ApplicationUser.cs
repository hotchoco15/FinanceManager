using Microsoft.AspNetCore.Identity;
using System;


namespace Entities.IdentityEntities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		public string? PersonName { get; set; }
	}
}
