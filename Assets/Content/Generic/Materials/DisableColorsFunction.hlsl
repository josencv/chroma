#define IF(a, b, c) lerp(b, c, step((float) (a), 0))
#define EPSILON 1e-10

float3 HUEtoRGB(in float H)
{
  float R = abs(H * 6 - 3) - 1;
  float G = 2 - abs(H * 6 - 2);
  float B = 2 - abs(H * 6 - 4);
  return saturate(float3(R, G, B));
}

float3 RGBtoHCV(in float3 RGB)
{
  // Based on work by Sam Hocevar and Emil Persson
  float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
  float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
  float C = Q.x - min(Q.w, Q.y);
  float H = abs((Q.w - Q.y) / (6 * C + EPSILON) + Q.z);
  return float3(H, C, Q.x);
}

float3 HSVtoRGB(in float3 HSV)
{
  float3 RGB = HUEtoRGB(HSV.x);
  return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float3 RGBtoHSV(in float3 RGB)
{
  float3 HCV = RGBtoHCV(RGB);
  float S = HCV.y / (HCV.z + EPSILON);
  return float3(HCV.x, S, HCV.z);
}

float3 desaturate(float3 hsv) {
  hsv.y = 0;
  return hsv;
}

void DisableColors_float(float4 albedo, Texture2D<float4> absorptionData, out float4 output) {
  float _DisableRed = absorptionData.Load(int3(0, 31, 0));
  float _DisableYellow = absorptionData.Load(int3(1, 31, 0));
  float _DisableGreen = absorptionData.Load(int3(2, 31, 0));
  float _DisableCyan = absorptionData.Load(int3(3, 31, 0));
  float _DisableBlue = absorptionData.Load(int3(4, 31, 0));
  float _DisableMagenta = absorptionData.Load(int3(5, 31, 0));

  float3 hsv = RGBtoHSV(albedo.rgb);

  // TODO: change to step and lerp to avoid branching
  if (_DisableYellow == 1 && hsv.x >= 0.006 && hsv.x < 0.28)
  {
    hsv = desaturate(hsv);
  }
  else if (_DisableGreen == 1 && hsv.x >= 0.28 && hsv.x < 0.37)
  {
    hsv = desaturate(hsv);
  }
  else if (_DisableCyan == 1 && hsv.x >= 0.37 && hsv.x < 0.61)
  {
    hsv = desaturate(hsv);
  }
  else if (_DisableBlue == 1 && hsv.x >= 0.61 && hsv.x < 0.70)
  {
    hsv = desaturate(hsv);
  }
  else if (_DisableMagenta == 1 && hsv.x >= 0.70 && hsv.x < 0.95)
  {
    hsv = desaturate(hsv);
  }
  else if (_DisableRed == 1 && (hsv.x >= 0.95 || hsv.x < 0.006))
  {
    hsv = desaturate(hsv);
  }

  float3 rgb = HSVtoRGB(hsv);

  output = float4(rgb, albedo.a);
}
