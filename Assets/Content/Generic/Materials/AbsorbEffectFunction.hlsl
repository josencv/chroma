#define IF(a, b, c) lerp(b, c, step((float) (a), 0))

void AbsorbEffect_float(float4 albedo, float3 position, float4 effectPosition, float radius, out float4 output) {
  albedo = IF(
    distance(position, effectPosition.xyz) < radius,
    lerp(albedo.r * 0.299 + albedo.g * 0.587 + albedo.b * 0.114, albedo, 0),
    albedo
  );

  output = albedo;
}
