# SuSuerteV2

## Configuración de seguridad

### Configuración de la clave API de Mailgun

Por razones de seguridad, la clave API de Mailgun ya no se almacena directamente en el archivo App.config. El sistema está configurado para funcionar de las siguientes maneras:

#### Para desarrollo local

Para desarrollo local, crea un archivo llamado `mailgun_key.txt` en el directorio de la aplicación (bin/Debug o bin/Release) con tu clave API de Mailgun. Este archivo no se incluirá en el repositorio Git.

```
tu-clave-api-aqui
```

Este enfoque te permite desarrollar y probar localmente sin exponer la clave en el código fuente.

#### Para entornos de producción (recomendado)

En entornos de producción, se recomienda usar variables de entorno para mayor seguridad:

1. Configura una variable de entorno llamada `MAILGUN_API_KEY` con tu clave API de Mailgun.

   En Windows Server, puedes hacerlo desde PowerShell con el siguiente comando:
   ```powershell
   [Environment]::SetEnvironmentVariable("MAILGUN_API_KEY", "tu-clave-api-aquí", "Machine")
   ```

   O desde el Panel de Control:
   - Busca "Variables de entorno" en el menú de inicio
   - Haz clic en "Editar las variables de entorno del sistema"
   - En la pestaña "Avanzado", haz clic en "Variables de entorno"
   - Añade una nueva variable del sistema con nombre `MAILGUN_API_KEY` y el valor de tu clave API

2. Reinicia el servidor web o la aplicación para que reconozca la nueva variable de entorno.

#### Para IIS o servicios web

Si estás desplegando en IIS, puedes configurar la variable de entorno en el archivo web.config o en la configuración del sitio en IIS:

```xml
<configuration>
  <system.webServer>
    <environmentVariables>
      <add name="MAILGUN_API_KEY" value="tu-clave-api-aquí" />
    </environmentVariables>
  </system.webServer>
</configuration>
```

#### Usando transformaciones de configuración (alternativa)

También puedes usar el archivo `App.Release.config` incluido en el proyecto. Este archivo se aplicará automáticamente cuando compiles en modo Release. Solo necesitas editar este archivo y reemplazar `TU_CLAVE_API_REAL_AQUI` con tu clave API real.

Esta configuración evita que las claves API se expongan en el código fuente y en los repositorios Git, mientras mantiene la funcionalidad en todos los entornos.