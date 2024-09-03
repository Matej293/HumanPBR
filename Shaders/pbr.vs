#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;
out vec3 WorldPos;
out vec3 Normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat3 normalMatrix;

uniform sampler2D displacementMap;
uniform float displacementIntensity = 1.0f;
uniform float displacementThreshold = 0.5f;

void main()
{
    // Pass through UV coordinates
    TexCoords = aTexCoords;

    // Calculate initial normal in world space
    vec3 normal = normalize(normalMatrix * aNormal);

    // Sample the displacement map using the UV coordinates
    vec4 dv = texture(displacementMap, aTexCoords);

    // Convert the sampled color to a grayscale value (luminance) for displacement
    float df = 0.30 * dv.r + 0.59 * dv.g + 0.11 * dv.b;

    // Apply displacement intensity and threshold
    float displacementAmount = df * displacementIntensity;
    if (displacementAmount < displacementThreshold) {
        displacementAmount = 0.0; // Ignore displacement if below threshold
    }

    // Displace the vertex position along the normal direction
    vec3 displacedPos = aPos + normal * displacementAmount;

    // Transform the displaced position to world space
    WorldPos = vec3(model * vec4(displacedPos, 1.0));

    // Transform the normal to world space
    Normal = normalize(normalMatrix * aNormal);

    // Compute final position in clip space
    gl_Position = projection * view * vec4(WorldPos, 1.0);
}
