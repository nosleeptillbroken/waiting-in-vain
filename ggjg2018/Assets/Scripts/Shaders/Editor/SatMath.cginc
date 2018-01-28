#if !defined(SAT_MATH)
#define SAT_MATH

float rand(float2 co)
{
	return frac(sin(1000.0*dot(co.xy, float2(21.5739, 43.421))) * 617284.3);
}

float rand(float co)
{
	return frac(sin(1000.0 * co) * 617284.3);
}

#endif