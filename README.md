# NULLSCAPE 2D
### Trabajo Final Integrador — Programación de Videojuegos I

<p align="center">
  **Un roguelike de plataformas 2D con generación procedural, enemigos progresivos y supervivencia.**
</p>

<img width="655" height="303" alt="image" src="https://github.com/user-attachments/assets/dfae1d70-b881-4830-a0c8-5c4398dfcd98" />

---

## 📖 Descripción del proyecto

**NullScape 2D** es un videojuego de plataformas y supervivencia inspirado en la experiencia de *NullScape* de Roblox. El jugador debe sobrevivir durante la mayor cantidad de niveles posible, enfrentándose a una dificultad que aumenta progresivamente.

Cada nivel presenta una distribución procedural de plataformas y desafíos. Para avanzar, el jugador debe recolectar todos los **regalos negros**, que funcionan como objetivo obligatorio. Una vez recolectados, se desbloquea el portal que permite continuar al siguiente nivel.

Durante la exploración también aparecen regalos dorados, cuya recolección es opcional y permite conseguir oro para utilizar en las tiendas.

Sin embargo, el jugador no estará solo: deberá esquivar enemigos que lo persiguen, enemigos a distancia que disparan proyectiles y distintas amenazas que se vuelven cada vez más peligrosas.

Entre niveles, el jugador puede encontrarse con distintas tiendas donde comprar mejoras, seleccionar enemigos adicionales o adquirir maldiciones que modifican y potencian a los enemigos.

La partida termina cuando el jugador pierde todas sus vidas. Al morir, pierde el progreso de la partida y debe comenzar una nueva desde el menú principal.

**El objetivo es simple: sobrevivir, mejorar, administrar los recursos y llegar tan lejos como sea posible.**

---

## 🎮 Características principales

- Generación procedural de niveles.
- Dificultad progresiva a medida que aumenta el nivel.
- Plataformas de distintos tipos y distribuciones.
- Recolección obligatoria de regalos negros.
- Regalos dorados opcionales para conseguir oro.
- Portal de salida que se desbloquea al completar el objetivo.
- Sistema de enemigos acumulativos.
- Enemigos básicos que persiguen al jugador.
- Enemigos a distancia que persiguen y disparan proyectiles.
- Sistema de maldiciones que modifica las estadísticas de los enemigos.
- Tiendas de mejoras, enemigos y maldiciones.
- Sistema de corazones y escudos.
- Mejoras permanentes de estadísticas durante la partida.
- Inventario con categorías y acumulación de objetos.
- Sistema de pausa y Game Over.
- Animaciones mediante sprites.
- Música y efectos de sonido.
- Configuración del volumen de música desde el menú principal.

---

## 🕹️ Controles

| Tecla | Acción |
|---|---|
| A | Moverse hacia la izquierda |
| D | Moverse hacia la derecha |
| W | Movimiento hacia arriba, según la mecánica del juego |
| S | Movimiento hacia abajo, según la mecánica del juego |
| SPACE | Saltar |
| M | Abrir / cerrar inventario |
| ESC | Pausar / reanudar partida |

---

## 🏆 Objetivo y progresión

### 1. Generación de niveles

Los escenarios se generan proceduralmente, modificando la distribución y cantidad de plataformas a medida que el jugador avanza.

El sistema utiliza diferentes tipos de baldosas y plataformas para construir recorridos variables, evitando que todos los niveles tengan exactamente la misma distribución.

<img width="762" height="338" alt="image" src="https://github.com/user-attachments/assets/00a64686-3a97-4192-972e-2716fac75fb8" />
<img width="757" height="330" alt="image" src="https://github.com/user-attachments/assets/56ecf68d-9839-4a60-9aa1-f4b041e7eadd" />
<img width="780" height="341" alt="image" src="https://github.com/user-attachments/assets/1acea131-60de-492f-bb38-84f319065845" />

### 2. Regalos negros: objetivo obligatorio

Los regalos negros son los coleccionables principales de cada nivel.

El jugador debe encontrar y recolectar todos los regalos negros para completar el objetivo. El HUD muestra el progreso de recolección.

Cuando se consigue el último regalo negro, se desbloquea el portal de salida.

<img width="533" height="113" alt="image" src="https://github.com/user-attachments/assets/ae028b7e-bdec-4a10-b549-8f930e282f61" />


### 3. Regalos dorados: recompensa opcional

Una vez recolectados todos los regalos negros, aparecen los regalos dorados.

Estos no son obligatorios para avanzar, pero permiten conseguir oro adicional para gastar en las tiendas.

El jugador debe decidir cuánto tiempo quiere dedicar a recolectarlos y cuánto riesgo está dispuesto a asumir.

<img width="518" height="293" alt="image" src="https://github.com/user-attachments/assets/60f50595-ac28-46a5-bd3f-40a130ce9089" />


### 4. Portal de salida

Cuando se completa la recolección obligatoria, el portal cambia visualmente de estado y queda habilitado para salir del nivel.

Al entrar, se completa la etapa y se ejecuta el sistema de progresión, que determina si corresponde generar otro nivel o enviar al jugador a una tienda.

<img width="356" height="264" alt="image" src="https://github.com/user-attachments/assets/ad809a48-daab-4433-b1a0-772d239b0332" />

---

<img width="458" height="288" alt="image" src="https://github.com/user-attachments/assets/1c5af9a5-e3d8-4e64-9367-6abf7c1a80f9" />

## 👾 Enemigos e inteligencia artificial

El juego cuenta con diferentes tipos de enemigos que representan amenazas distintas.

### Enemigo básico

- Persigue continuamente al jugador.
- Se desplaza utilizando físicas 2D.
- Inflige daño al entrar en contacto con el jugador.
- Respeta los tiempos de inmunidad del sistema de vida.
- Cuenta con animación Idle mediante sprites.


<img width="267" height="167" alt="image" src="https://github.com/user-attachments/assets/f570b943-b0e1-46f8-afdc-4a41858ac7d8" />


### Enemigo a distancia

- Persigue al jugador.
- Detecta cuándo se encuentra dentro de su rango de ataque.
- Dispara proyectiles hacia el jugador.
- Puede disparar varios proyectiles por ráfaga.
- Permite configurar la cantidad de proyectiles y el intervalo entre disparos.
- Tiene un tiempo de cooldown antes de volver a atacar.
- Cuenta con conjuntos de sprites independientes para el ataque y el tiempo de espera.

<!-- CAPTURA: ENEMIGO A DISTANCIA -->
<img width="570" height="128" alt="image" src="https://github.com/user-attachments/assets/b839d785-c631-4013-ab14-6463b6617cdc" />
<img width="245" height="158" alt="image" src="https://github.com/user-attachments/assets/c0bdbb35-d4c6-4c04-9143-46d6c945b3aa" />
(por una configuración desconocida, el sprite se renderizó mal(?)

### Sistema de maldiciones

Las maldiciones son modificadores que aumentan o alteran las capacidades de los enemigos.

Pueden modificar sus estadísticas y hacer que los encuentros sean más complicados a medida que avanza la partida.

Las maldiciones adquiridas se procesan mediante el sistema de inventario y pueden generar enemigos adicionales.

<!-- CAPTURA: TIENDA DE MALDICIONES -->
<img width="626" height="290" alt="image" src="https://github.com/user-attachments/assets/84e24d34-546f-43db-a0ab-b900e9f3e55e" />
<img width="602" height="191" alt="image" src="https://github.com/user-attachments/assets/858459dc-f99a-442c-9fba-ac2c297d420d" />
<img width="599" height="205" alt="image" src="https://github.com/user-attachments/assets/8a5280b3-cd63-4f22-9d58-37cb6930d9c9" />
<img width="577" height="192" alt="image" src="https://github.com/user-attachments/assets/d8f19abf-0304-4a45-8e59-f2ff8ca08f4e" />


### Sistema de aparición de enemigos

El `EnemySpawner2D` utiliza los datos del inventario y las definiciones de enemigos para generar las instancias correspondientes.

También procesa los enemigos adicionales asociados a las maldiciones.

Esto permite que las amenazas se acumulen y que el jugador tenga que adaptarse constantemente.

---

## 🛒 Tiendas y economía

Entre los niveles, el jugador puede acceder a distintas tiendas.

### Tienda principal

Permite gastar el oro acumulado para comprar mejoras que modifican las estadísticas del personaje.

### Tienda de enemigos

Permite seleccionar y adquirir nuevos enemigos que pasarán a formar parte de los desafíos de los siguientes niveles.

Incluye selección de enemigos, sistema de reroll y límites de acumulación.

### Tienda de maldiciones

Permite adquirir modificadores que potencian a los enemigos y aumentan la dificultad de la partida.

### Economía

El sistema utiliza oro como recurso principal.

El jugador puede conseguirlo recolectando regalos dorados y aprovechar los multiplicadores de oro disponibles.

<img width="655" height="298" alt="image" src="https://github.com/user-attachments/assets/98ab97b8-e29f-4a28-9f27-56dca2ee1794" />
<img width="508" height="163" alt="image" src="https://github.com/user-attachments/assets/6eeaee3d-d1cc-4cd1-8b2a-2b6e41d91022" />

<img width="562" height="305" alt="image" src="https://github.com/user-attachments/assets/60f962c2-74d7-46ac-bb4c-b3cb181dea5f" />

![Tienda de enemigos](Capturas/tienda-enemigos.png)


<img width="577" height="192" alt="image" src="https://github.com/user-attachments/assets/982d9ef1-66e1-4d6f-aa24-20bb1b4eaaff" />


---

## ❤️ Sistema de vida, corazones y escudos

El personaje cuenta con un sistema de supervivencia basado en corazones y escudos.

- Los corazones representan la vida del jugador.
- Los escudos absorben daño antes de que se pierdan corazones.
- El sistema permite aumentar las cantidades mediante mejoras.
- Al recibir daño se aplica un tiempo de inmunidad para evitar impactos consecutivos.
- Al perder todos los corazones se activa el Game Over.

Los indicadores se muestran en pantalla y están preparados para utilizar iconos y sprites.

<img width="130" height="56" alt="image" src="https://github.com/user-attachments/assets/357636d0-4ad9-4ae8-a842-97b8d4a5ef6b" />

<img width="86" height="59" alt="image" src="https://github.com/user-attachments/assets/832a49c6-7c1d-46d6-9c6c-346c373dde2a" />

---

## 📦 Inventario y mejoras del personaje

El inventario permite visualizar los objetos, enemigos y maldiciones que el jugador ha adquirido.

Se encuentra organizado en tres categorías:

- **Items:** mejoras y objetos del jugador.
- **Enemigos:** enemigos adquiridos.
- **Maldiciones:** modificadores de los enemigos.

Los elementos iguales se agrupan y muestran su cantidad acumulada.

El sistema de mejoras permite modificar diferentes atributos del jugador, entre ellos:

- Velocidad de movimiento.
- Tiempo de aceleración.
- Tiempo de desaceleración.
- Gravedad.
- Fuerza de salto.
- Cantidad de saltos adicionales.
- Corazones adicionales.
- Escudos adicionales.

Las estadísticas se calculan a partir de los valores base, permitiendo combinar varias mejoras sin perder la configuración original del personaje.

<img width="610" height="181" alt="image" src="https://github.com/user-attachments/assets/5e0c3015-0ff5-4b1a-a160-44705b7d399a" />

---

## 🎨 Animaciones y apartado visual

El proyecto utiliza sprites 2D para representar las animaciones de los personajes y elementos del escenario.

### Jugador

- Idle: animación de reposo.
- Walk: animación de movimiento.
- Jump: sprite durante el ascenso.
- Fall: sprite durante la caída.
- Orientación horizontal mediante FlipX.

La cantidad de sprites y los intervalos de reproducción se pueden configurar desde el Inspector.

### Enemigos

Los enemigos cuentan con animaciones configurables mediante arreglos de sprites e intervalos de reproducción.

El enemigo básico utiliza una animación de reposo, mientras que el enemigo a distancia diferencia visualmente sus estados de ataque y cooldown.

### Portal

El portal cuenta con dos sprites diferentes:

- Cerrado: mientras el objetivo del nivel no se haya completado.
- Abierto: cuando se recolectaron todos los regalos negros.

### Escenario

Los niveles utilizan diferentes plataformas y baldosas, junto con una cámara y un fondo negro para reforzar la estética visual del juego.

<img width="492" height="162" alt="image" src="https://github.com/user-attachments/assets/3390f612-fd9c-47e7-9af6-55f0b23834b6" />

---

## 🔊 Sistema de audio

El juego cuenta con un `AudioManager` centralizado que administra la música y los efectos de sonido sin tener que implementar toda la lógica de audio en cada sistema de gameplay.

### Música por escenas

Cada sección del juego cuenta con su propia música:

- Menú principal.
- Gameplay.
- Tienda principal.
- Tienda de enemigos.
- Tienda de maldiciones.
- Pausa.
- Game Over.

### Progresión musical

La música del gameplay cambia según el progreso:

- **Niveles 1 al 9:** música correspondiente a las primeras etapas de la partida.
- **Nivel 10 en adelante:** canciones con una temática más intensa y orientada a una partida de supervivencia avanzada.

El sistema evita repetir inmediatamente la última pista reproducida.

### Efectos de sonido

Se incorporaron sonidos para distintas acciones:

- Salto.
- Pasos.
- Daño.
- Recolección de regalos negros.
- Recolección de regalos dorados.
- Aparición de regalos dorados.
- Apertura del portal.
- Botones de interfaz.
- Aviso de proximidad al portal.

El menú principal dispone de un panel de ajustes para configurar el volumen de la música.

<img width="507" height="261" alt="image" src="https://github.com/user-attachments/assets/d6111f59-92a0-4aac-a611-cfaf43ec7ccc" />


---

## 🧠 Arquitectura y sistemas de programación

El proyecto está desarrollado en C# utilizando Unity y una arquitectura basada en componentes, separación de responsabilidades y datos configurables.

A continuación se describen algunos de los principales sistemas:

| Script / Sistema | Responsabilidad |
|---|---|
| `GameManager` | Control central del flujo de partida, inicio y retorno al menú. |
| `PlayerController2D` | Movimiento, salto, físicas y animaciones del jugador. |
| `PlayerStats` | Configuración de estadísticas base del jugador mediante ScriptableObject. |
| `PlayerItemEffects` | Aplicación y acumulación de mejoras. |
| `PlayerVitals` | Gestión de corazones, escudos y daño. |
| `PlayerInventory` | Inventario y acumulación de elementos. |
| `PlayerScenePersistence2D` | Conservación de los datos del jugador entre escenas. |
| `LevelProgressManager2D` | Progresión de niveles y selección de tiendas. |
| `EnemySpawner2D` | Aparición de enemigos según inventario y maldiciones. |
| `BasicEnemyFollower2D` | Seguimiento y daño por contacto. |
| `DistanceEnemyFollower2D` | Seguimiento y ataques a distancia. |
| `EnemyRuntimeStats` | Aplicación de modificadores a las estadísticas enemigas. |
| `EnemyDefinition` | Configuración de los datos de cada enemigo. |
| `GiftProgressManager2D` | Control de regalos obligatorios y opcionales. |
| `ExitDoor2D` | Desbloqueo y transición al siguiente nivel. |
| `AudioManager` | Música, efectos de sonido y audio por escena. |
| `CurseShopManager2D` | Administración de la tienda de maldiciones. |

### Técnicas utilizadas

- `Rigidbody2D` para el movimiento y las físicas.
- `Physics2D` y `LayerMask` para detecciones y comprobaciones de contacto con el suelo.
- `ScriptableObjects` para separar datos de la lógica de juego.
- `Coroutine` para secuencias temporales, esperas y generación de elementos.
- Eventos C# para comunicar sistemas independientes.
- `SceneManager` para la transición entre escenas.
- `DontDestroyOnLoad` para conservar sistemas persistentes.
- `PlayerPrefs` para preferencias locales, como el volumen de música.
- `Time.timeScale` para controlar la pausa.
- `SpriteRenderer` para la representación y animación de sprites.

---

## 🗺️ Flujo general de juego

El flujo principal de una partida es el siguiente:

```text
                 MAIN MENU
                     |
                     v
               CREAR PARTIDA
                     |
                     v
                SAMPLE SCENE
                     |
                     v
             GENERACIÓN DEL NIVEL
                     |
                     v
           RECOLECTAR REGALOS NEGROS
                     |
                     v
           ¿SE RECOLECTARON TODOS?
                 /       \
               NO         SÍ
               |           |
               v           v
           CONTINUAR    APARECEN LOS
           EXPLORANDO   REGALOS DORADOS
                           |
                           v
                    RECOLECTAR ORO
                       (OPCIONAL)
                           |
                           v
                    PORTAL ABIERTO
                           |
                           v
                      SALIR DEL NIVEL
                           |
                           v
                 ¿CORRESPONDE UNA TIENDA?
                     /           \
                   SÍ             NO
                   |               |
                   v               v
                TIENDA       NUEVO NIVEL
                   |               |
                   +-------+-------+
                           |
                           v
                    CONTINUAR RUN
```

Si el jugador pierde todos sus corazones, se activa Game Over y la partida termina. Al comenzar una nueva partida, el progreso de la anterior se reinicia.


## 🛠️ Tecnologías utilizadas

| Tecnología | Uso |
|---|---|
| Unity 6 | Motor de desarrollo |
| C# | Programación de sistemas |
| Rigidbody2D | Físicas y movimiento |
| Physics2D | Detección y colisiones |
| ScriptableObjects | Datos configurables |
| uGUI | Interfaz de usuario |
| SpriteRenderer | Representación de sprites |
| AudioSource | Música y efectos |
| Git | Control de versiones |
| GitHub | Repositorio y colaboración |

**Versión exacta de Unity:** completar según la versión utilizada en el proyecto.

**Render Pipeline:** completar según la configuración del proyecto (Built-in o URP).

---

## 📸 Galería de capturas

Esta sección está preparada para documentar visualmente los distintos sistemas implementados.

| Captura | Archivo sugerido |
|---|---|
| Menú principal | `Capturas/main-menu.png` |
| Panel de clase | `Capturas/clase-panel.png` |
| Gameplay general | `Capturas/gameplay.png` |
| Generación procedural | `Capturas/generacion-procedural.png` |
| Jugador y animaciones | `Capturas/player.png` |
| Enemigo básico | `Capturas/enemigo-basico.png` |
| Enemigo a distancia | `Capturas/enemigo-distancia.png` |
| Regalos negros | `Capturas/regalos-negros.png` |
| Regalos dorados | `Capturas/regalos-dorados.png` |
| Portal | `Capturas/portal.png` |
| Tienda principal | `Capturas/tienda-principal.png` |
| Tienda de enemigos | `Capturas/tienda-enemigos.png` |
| Tienda de maldiciones | `Capturas/tienda-maldiciones.png` |
| Inventario | `Capturas/inventario.png` |
| Corazones y escudos | `Capturas/vida-escudos.png` |
| Menú de pausa | `Capturas/pausa.png` |
| Game Over | `Capturas/game-over.png` |

---

## 🚧 Desarrollo futuro y visión original

La idea original de NullScape 2D contempla una experiencia de supervivencia mucho más amplia.

El objetivo a futuro es ampliar el contenido y la variedad de desafíos mediante:

- Una mayor cantidad de enemigos y comportamientos.
- Nuevas maldiciones con efectos más variados.
- Habilidades únicas para el jugador.
- Diferentes clases de personaje con atributos y estilos de juego propios.
- Un jefe final que funcione como desafío definitivo.
- Más plataformas, escenarios y eventos especiales.
- Más objetos, mejoras y combinaciones posibles.

Estas características forman parte de la visión de expansión del proyecto y no deben confundirse con los sistemas que ya están implementados en la versión actual.

---

## 📋 Relación con el Trabajo Final Integrador

El desarrollo toma como referencia las consignas del Trabajo Final Integrador de Programación de Videojuegos I.

| Consigna del PDF | Aplicación en NullScape 2D |
|---|---|
| 1. Base del proyecto y GitHub | Proyecto Unity organizado y versionado mediante Git y GitHub. |
| 2. Control del jugador y físicas | Movimiento mediante Rigidbody2D, aceleración, desaceleración, salto y gravedad. |
| 3. Detección por físicas y capas | Comprobaciones de suelo y detecciones mediante Physics2D y LayerMask. |
| 4. Corrutinas e invocaciones | Aparición de enemigos, intervalos, esperas, cooldowns y secuencias temporales. |
| 5. Eventos y ScriptableObjects | PlayerStats, EnemyDefinition, datos de inventario y comunicación mediante eventos. |
| 6. GameManager y persistencia | Control de partida, pausa, cambios de escena y conservación de datos entre escenas. |
| 7. Inteligencia artificial | Enemigos seguidores y enemigos a distancia con persecución y ataque. |
| 8. Interfaz de usuario | HUD, inventario, tiendas, menús, contadores y paneles. |
| 9. Sistema de audio | AudioManager, música por escena, efectos y configuración de volumen de música. |
| 10. Escenarios y animaciones | Escenarios 2D con plataformas, generación procedural y animaciones mediante sprites. |
| 11. Build y Player Settings | Preparación de la compilación y configuración de publicación. |
| 12. itch.io | Preparación de la página pública y publicación de la versión jugable. |

### Consignas opcionales y ampliaciones

La implementación más directamente relacionada con las opciones del PDF es la **Opción A: Sistema de Tienda y Economía entre Niveles**, mediante el oro, las compras y las mejoras persistentes durante la partida.

Además, se desarrollaron sistemas complementarios de selección de enemigos y maldiciones, ampliando la variedad de compras y la dificultad de los encuentros.

---

## 📌 Aspectos pendientes de completar para la entrega

- [ ] Confirmar que el repositorio sea público y tenga el nombre requerido por la cátedra.
- [ ] Completar los datos de los integrantes y sus legajos.
- [ ] Documentar los roles y ramas de GitHub.
- [ ] Verificar el historial de commits y Pull Requests.
- [ ] Confirmar las escenas y el flujo de la Vertical Slice solicitada.
- [ ] Verificar los canales independientes Master, BGM y SFX del AudioMixer.
- [ ] Completar las configuraciones de Player Settings.
- [ ] Generar y probar la build WebGL.
- [ ] Publicar la página del proyecto en itch.io.
- [ ] Incorporar al menos dos capturas o GIFs reales del juego.
- [ ] Preparar el video demostrativo de 1 a 2 minutos.
- [ ] Completar los créditos del equipo.

---

## 👥 Integrantes y distribución de roles

| Integrante | Legajo | Rol principal | Rama |
|---|---|---|---|
| Nombre y apellido | Legajo | Programación de gameplay | `nombre-apellido` |
| Nombre y apellido | Legajo | Diseño y UI | `nombre-apellido` |



## 💡 Créditos y agradecimientos

Proyecto desarrollado para el **Trabajo Final Integrador de Programación de Videojuegos I**.

La propuesta busca demostrar la integración de programación orientada a componentes, físicas 2D, generación procedural, inteligencia artificial, sistemas de progresión, interfaz de usuario, audio y administración de datos dentro de una experiencia jugable.

**NULLSCAPE 2D — Sobreviví un nivel más.**

Grupo: Null

Integrantes:
- Ana Florencia Navarro
- Robert Fuentes Facundo Guillermo
