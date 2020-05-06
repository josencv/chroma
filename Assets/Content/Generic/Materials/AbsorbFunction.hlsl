#define IF(a, b, c) lerp(b, c, step((float) (a), 0))

void Absorb_float(float4 albedo, float3 position, Texture2D<float4> absorptionData, float absorptionDataLength, out float4 output) {
  for (int i=0; i<absorptionDataLength; i++) {
    float4 absorptionPos = absorptionData.Load(int3(i, 0, 0));
    float4 absorptionExtraData = absorptionData.Load(int3(i, 1, 0));
    float radius = absorptionExtraData.x;
    float saturation = absorptionExtraData.y;

    albedo = IF(
      distance(position, absorptionPos.xyz) < radius,
      lerp(albedo.r * 0.299 + albedo.g * 0.587 + albedo.b * 0.114, albedo, saturation),
      albedo
    );
  }

  output = albedo;
}
