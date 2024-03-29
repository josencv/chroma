void HueSaturationFilter_float(
    float4 Input,
    float Hue,
    float3 PlayerPos,
    float3 WorldPos,
    float Range,
    float Amount,
    float LowerLimit,
    float UpperLimit,
    out float4 Output,
    out float HueOut,
    out float3 PlayerPosOut,
    out float3 WorldPosOut,
    out float RangeOut
)
    {

        float dist = distance(WorldPos, PlayerPos);
        float absDist = abs(dist);
        absDist /= Range;

        float powDist = pow(pow(absDist, 7), 2);
        Amount += powDist;

        Amount = clamp(Amount, 0.0, 1.0);

        LowerLimit /= 360.0;
        UpperLimit /= 360.0;

        float difference = (UpperLimit - LowerLimit) * 1.25;

        float limitMean = (UpperLimit + LowerLimit) * 0.5;
        float absDistance = abs(Hue - limitMean);

        float hueMask = smoothstep(min(0.035, difference), difference, absDistance) + Amount;

        hueMask = clamp(hueMask, 0.0, 1.0);

        float luma = dot(Input, float3(0.2126729, 0.7151522, 0.0721750));

        Output = luma.xxxx + hueMask.xxxx * (Input - luma.xxxx);
        HueOut = Hue;
        PlayerPosOut = PlayerPos;
        WorldPosOut = WorldPos;
        RangeOut = Range;
    }