#version 460 core
out vec4 FragColor;
in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

// Material parameters
uniform sampler2D albedoMap;
uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform sampler2D roughnessMap;
uniform sampler2D sssMap; // Subsurface scattering map

// IBL parameters
uniform samplerCube irradianceMap;
uniform samplerCube prefilterMap;
uniform sampler2D brdfLUT;

// Lights
uniform vec3 lightPositions[4];
uniform vec3 lightColors[4];
uniform vec3 camPos;

const float PI = 3.14159265359;

// ----------------------------------------------------------------------------
// Function to get the normal from the normal map and transform it to world space.
vec3 getNormalFromMap()
{
    vec3 tangentNormal = texture(normalMap, TexCoords).xyz * 2.0 - 1.0;
    
    vec3 N = normalize(Normal);
    vec3 T = vec3(1.0, 0.0, 0.0); // Placeholder tangent
    vec3 B = vec3(0.0, 1.0, 0.0); // Placeholder bitangent
    mat3 TBN = mat3(T, B, N);
    
    return normalize(TBN * tangentNormal);
}

// ----------------------------------------------------------------------------
// Function for Fresnel-Schlick approximation
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (vec3(1.0) - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

// ----------------------------------------------------------------------------
// Function for basic Lambertian diffuse
vec3 lambertDiffuse(vec3 albedo)
{
    return albedo / PI;
}

// ----------------------------------------------------------------------------
// Function for Blinn-Phong specular reflection
float blinnPhongSpecular(float NdotH, float roughness)
{
    return pow(NdotH, 1.0 / roughness);
}

// ----------------------------------------------------------------------------
// Main rendering function
void main()
{
    // Material properties
    vec3 albedo = texture(albedoMap, TexCoords).rgb;
    float roughness = texture(roughnessMap, TexCoords).r;
    float sssIntensity = texture(sssMap, TexCoords).r; // Subsurface scattering intensity
    vec3 specularColor = texture(specularMap, TexCoords).rgb;
    
    // Input lighting data
    vec3 N = getNormalFromMap();
    vec3 V = normalize(camPos - WorldPos);
    vec3 R = reflect(-V, N); 

    // Calculate reflectance at normal incidence (Fresnel term)
    vec3 F0 = specularColor; // Specular color determines F0 for dielectric materials

    // IBL components
    vec3 irradiance = texture(irradianceMap, N).rgb;
    vec3 prefilteredColor = textureLod(prefilterMap, R, roughness * 4.0).rgb;
    vec2 brdf = texture(brdfLUT, vec2(max(dot(N, V), 0.0), roughness)).rg;

    // Diffuse and specular contributions
    vec3 diffuse = lambertDiffuse(albedo);
    vec3 specular = prefilteredColor * (F0 * brdf.x + brdf.y);

    // Calculate direct lighting
    vec3 Lo = vec3(0.0);
    for(int i = 0; i < 4; ++i) 
    {
        // Calculate per-light radiance
        vec3 L = normalize(lightPositions[i] - WorldPos);
        vec3 H = normalize(V + L);
        float distance = length(lightPositions[i] - WorldPos);
        float attenuation = 1.0 / (distance * distance);
        vec3 radiance = lightColors[i] * attenuation;

        // Basic Lambertian diffuse
        float NdotL = max(dot(N, L), 0.0);

        // Blinn-Phong specular
        float NdotH = max(dot(N, H), 0.0);
        float specularContribution = blinnPhongSpecular(NdotH, roughness);

        // Add to outgoing radiance Lo
        Lo += (diffuse + specularColor * specularContribution) * radiance * NdotL;
    }   
    
    // Ambient lighting (IBL)
    vec3 ambient = (diffuse * irradiance + specular) * 0.1; // Simple ambient scaling
    
    // Combine ambient and direct lighting
    vec3 color = ambient + Lo;

    // Apply subsurface scattering
    vec3 sssColor = vec3(1.0, 0.8, 0.7); // SSS color tint for the sclera or iris
    vec3 sssDiffuse = sssColor * max(dot(N, V), 0.0);
    color = mix(color, sssDiffuse, sssIntensity);

    // Gamma correction
    color = pow(color, vec3(1.0 / 2.2)); 

    FragColor = vec4(color, 1.0);
}
