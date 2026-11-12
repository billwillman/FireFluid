public static class Interpolators
{
	public static LinearInterpolator linear = new LinearInterpolator();

	public static PolynomInterpolator quadratic = new PolynomInterpolator(2f);

	public static PolynomInterpolator squareRoot = new PolynomInterpolator(0.5f);

	public static PolynomInterpolator fourthRoot = new PolynomInterpolator(0.25f);

	public static PolynomInterpolator tenthRoot = new PolynomInterpolator(0.1f);

	public static PolynomInterpolator twentiethRoot = new PolynomInterpolator(0.05f);

	public static PolynomInterpolator hundredthRoot = new PolynomInterpolator(0.01f);
}
