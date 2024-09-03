#version 460 core

in vec3 FragPos;
in vec2 TexCoords;
in vec3 Normal;

out vec4 FragColor;

uniform sampler2D normalMap;
uniform float refractionIndex;
uniform vec3 cameraPos;

void main()
{
    vec3 normalFromMap = texture(normalMap, TexCoords).rgb;
    normalFromMap = normalize(normalFromMap * 2.0 - 1.0);

    vec3 viewDir = normalize(cameraPos - FragPos);

    // calculate refraction direction using Snell's law
    vec3 refractDir = refract(viewDir, normalFromMap, 1.0 / refractionIndex);

    // simulate refracted color as a simple background color
    vec3 backgroundColor = vec3(0.5, 0.7, 1.0);

    vec3 refractedColor = backgroundColor * (0.5 + 0.5 * dot(refractDir, normalFromMap));

    float transparency = 0.05;

    FragColor = vec4(refractedColor, transparency);
}