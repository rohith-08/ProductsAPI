using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ProductsAPI.Application.DTOs;

namespace ProductsAPI.Application.Validators
{
    public class RegisterValidator: AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Username)
           .NotEmpty().WithMessage("Username is required.")
           .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
           .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is REquired.")
                .EmailAddress().WithMessage("A valid email is requaried.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }

        public class LoginValidator:AbstractValidator<LoginDto>
        {
            public LoginValidator()
            {

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("A valid email is required.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.");

            }

        }
}}
