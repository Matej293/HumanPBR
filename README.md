# Physically Based Rendering in OpenGL

## Authors
<div align=justify> 
This project was made by two students, <em>Josip Cestar</em> and <i>Matej Spajić</i>, of the Undergraduate program in Mathematics and Computer Science at the School of Applied Mathematics and Informatics in Osijek  under the guidance of their mentor <em>doc. dr. sc. Domagoj Ševerdija</em>.
</div>

## Introduction

<div align=justify> 
Physically Based Rendering (PBR) is a method of shading and rendering 3D models that accurately represent how light interacts with surfaces and object materials.  The main purpose of the PBR technique is to accurately reproduce how light behaves in the real world, as opposed to earlier methods like the Phong and Blinn-Phong lighting models. Aside from its visual realism, PBR grants artists practical benefits and a convenient way of defining materials by physical properties, which reduces the use of workaround techniques and ‘lighting trickery.’ The project presents advanced rendering techniques and demonstrates the implementation of PBR using OpenGL.
</div>
<p></p>
<div style="display: flex; flex-wrap: wrap; justify-content:space-around; margin-top:1em;">
  <img src="https://i.imgur.com/3nkMk86.jpeg" alt="1" style="width: 49%; margin-bottom:4px;">
  <img src="https://i.imgur.com/lzTPuP7.jpeg" alt="2" style="width: 49%; margin-bottom:4px;">
  <img src="https://i.imgur.com/VzBslzF.jpeg" alt="3" style="width: 49%; margin-bottom:1em;">
  <img src="https://i.imgur.com/H34lxth.jpeg" alt="4" style="width: 49%; margin-bottom:1em;">
</div>

## Features

- **Realistic Material Rendering**: Supports PBR materials with albedo, normal, roughness, metallic, specular, ambient occlusion, and displacement maps.
- **Dynamic Lighting**: Allows for multiple light sources with configurable colors and positions.
- **Environment Mapping**: Converts HDR equirectangular environment maps to cubemaps for reflective materials.
- **Irradiance Mapping**: Computes the diffuse light interaction of complex surfaces.
- **Prefiltered Environment Map**: Utilizes a prefiltered environment map for correct specular highlights.
- **Brdf LUT Generation**: Generates a look-up table (LUT) for bidirectional reflectance distribution functions (BRDF).


## Prerequisites

### Windows
#### Prerequisites
- Windows 10
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- Recommended AMD GPU Architecture (also works on NVIDIA graphics cards)

## Installation

1. Clone the repository and navigate to the project directory:
    ```sh
    git clone https://github.com/Matej293/HumanPBR.git
    cd HumanPBR
    ```

2. Build the project using Visual Studio (ISO C++ 17 Standard) using prebuilt libraries. All the required ```dlls```, ```libs```, and ```includes``` can be found in the ```./Libraries/``` directory. The solution file can be found in the root folder of the project ```./HumanPBR.sln```. If you encounter issues regarding libraries, compile the libraries by yourself using [CMake](https://cmake.org/download/) since some pre-compiled libraries don't always work on all systems.


## Third party libraries

This project makes use of the following open source libraries:
- OpenGL 3.3 or higher (version used in this project is OpenGL 4.6)
- C++17 or higher
- [GLFW](https://www.glfw.org/)
- [GLAD](https://github.com/Dav1dde/glad)
- [stb_image](https://github.com/nothings/stb)
- [glm](https://github.com/g-truc/glm)
- [Open Asset Import Library](https://github.com/assimp/assimp/tree/master)

# How to use

## Controls
| Input | Action        |
| ----- | ------------- |
| Mouse | Rotate camera |
| W     | Move forward  |
| A     | Move left     |
| S     | Move right    |
| D     | Move backward |
| Esc   | Exit          |

## Technical Details

### Rendering Pipeline
The rendering pipeline of this project is structured to follow the modern OpenGL approach, utilizing shaders for handling the graphical output. The pipeline performs the following major steps:

1. **Initialization**: Initializes GLFW for window management and GLAD for OpenGL function loading. Sets up OpenGL context and required buffers.

2. **Shader Management**: Shaders are compiled and linked into program objects separate for different rendering tasks. Each shader handles specific responsibilities:
   - **PBR Shader**: Computes the final color of materials based on physical formulas, taking into account the light interactions, material properties, and camera position.
   - **Background Shader**: Renders the background using the environment cubemap.
   - **Irradiance Shader**: Computes the diffuse lighting contribution from the environment map.
   - **Prefilter Shader**: Prepares a map for specular highlights based on selected roughness levels.

3. **Model and Texture Loading**: 3D models are loaded using Assimp, and various textures (albedo, normal, specular, roughness, ambient occlusion, and displacement) are loaded using the STB image library.

4. **Environment Mapping**: 
    - **HDR Texture Loading**: An HDR equirectangular image is loaded, processed, and converted into a cubemap to allow for realistic reflective environments.
    - **Cubemap Generation**: The environment map is captured into a cubemap using predefined views to cover all six faces.

### Physically Based Rendering
The core of the project revolves around Physically Based Rendering principles:

- **Material Properties**: Each material is represented through a set of textures:
    - **Albedo**: Base color of the material.
    - **Normal Map**: Contains normals to create surface detail without adding more geometry.
    - **Roughness**: Defines how rough or smooth the surface appears, impacting the spread of reflection.
    - **Specular Map**: Controls the intensity of specular highlights.
    - **Ambient Occlusion**: Enhances shadows in crevices and provides depth.

- **Lighting Model**: The PBR shader computes lighting using both direct and indirect light. The illumination model is based on the Cook-Torrance BRDF (Bidirectional Reflectance Distribution Function):
    - **Fresnel Effect**: Simulates how light reflects off surfaces at glancing angles, enhancing realism.
    - **Microfacet Theory**: Models surface roughness by averaging the reflections of thousands of tiny facets, calculating specular reflection accordingly.

### Framebuffer Objects (FBOs)
The project utilizes Framebuffer Objects (FBOs) for off-screen rendering:
- **Irradiance Capture**: Off-screen FBOs gather light information from the environment map and generate the irradiance map used in rendering.
- **Prefiltered Environment Map**: Another FBO is used to create a prefiltered map for different levels of roughness, allowing for dynamic reflections depending on viewer and light positioning.

### Camera Control
A customizable camera system is implemented, allowing for smooth navigation and transformation within the scene:
- The camera uses matrices (view and projection) to manage the position and orientation, with support for user input to control movement and zoom.

### Performance Considerations
- The project aims to maintain a good balance between performance and visual fidelity:
    - **Mipmapping**: Used for textures to provide appropriate detail levels based on camera distance.
    - **Shader Optimization**: Complex calculations in shaders are structured to minimize computational load while providing realistic outputs.

### Rendering Loop
The main rendering loop handles:
1. **Input Processing**: Captures user inputs to adjust camera position and settings dynamically.
2. **Scene Rendering**: Draws the models, applying material properties and lighting in the PBR shader.
3. **Final Output**: Swaps the back buffer to present the rendered images to the screen.

The described architecture provides a general and scalable solution for achieving realistic rendering of 3D scenes. Further development may look into other material options, different lighting settings, and kinds of rasterization in order to expand the potential of the project.

## Included assets

The following assets are bundled with the project:
- [3D head model](https://www.3dscanstore.com/blog/Free-3D-Head-Model)
- HDR Map by Bernhard Vogl ["At the Window" (Wells, UK)](https://dativ.at/lightprobes/)

## References

- **A23D** ["Different Maps in PBR Textures."](https://www.a23d.co/blog/different-maps-in-pbr-textures)

- **Eugene d'Eon** ["NVIDIA Demo Team Secrets–Advanced Skin Rendering." Presentation at Game Developer Conference 2007.](http://developer.download.nvidia.com/presentations/2007/gdc/Advanced_Skin.pdf)

- **Heidrich, Wolfgang, and Eugene d'Eon** ["Advanced Techniques for Realistic Real-Time Skin Rendering." In *GPU Gems 3*, edited by Hubert Nguyen, 319-342. Springer, 2008.](https://link.springer.com/chapter/10.1007/978-3-642-03452-7_1)

- **Joey de Vries** [*Learn OpenGL*.](https://learnopengl.com/)

- **NVIDIA** ["Advanced Techniques for Realistic Real-Time Skin Rendering." In *GPU Gems 3*.](https://developer.nvidia.com/gpugems/gpugems3/part-iii-rendering/chapter-14-advanced-techniques-realistic-real-time-skin)

- **Sébastien Lagarde** ["Adopting a physically based shading model"](https://seblagarde.wordpress.com/2011/08/17/hello-world/)
