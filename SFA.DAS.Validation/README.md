# SFA.DAS.Validation

Automatic model validation for MVC applications that are using the OMIOMO (one model in, one model out) & PRG (Post/Redirect/Get) patterns.

## Configuration

### MVC Core

```c#
services.AddMvc(o => o.AddValidation());
```

### MVC

```c#
filters.AddValidationFilter();
```