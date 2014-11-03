This code shows four methods of rendering 3D scenes into images:
 1. CPU based Simulation Rendering: Raytracer implemented in CPU
 2. GPU based Simulation Rendering: Raytracer implemented in GPU
 3. CPU based Transformation Rendering: Rasterizer implemented in CPU
 4. GPU based Transformation Rendering: Rasterizer implemented in GPU

Options:
 
- You can select the type of rendering pressing the number associated to each renderer. Renderers 1,3 and 4 are syncrhonized
and show the same scene. Renderer 2 shows the scene described in its fragment shader.

- Use wasd keys to move the camera, and ijkl to move the light in the renderering modes 1,3 and 4.
- Use mouse to move the light position in rendering mode 2

Configuration:

The file config.xml contains general configuration for the renderers including the current scene. The base scene is the Cornell Box,
a standard scene used as an example in graphics.

The scene file supports:
 - multiple lights
 - multiple cameras
 - triangle objects
 - sphere objects
 - .obj meshes

See the corbellBox.xml file for an example of how to define a scene 

About:

This code is used in the course Introduction to Computer Graphics at the Computer Science Department of the Pontificia Universidad Catolica
of Chile and has been constantly been improved since the first version of the course in 2009.

Alejandro Echeverria 2014.