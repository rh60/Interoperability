#pragma once
#include <vector>
#include <array>

typedef std::vector<double> vec;
typedef std::array<double, 2> point;

class BSpline
{
	vec U;
	size_t p;

public:
	BSpline(vec);
	~BSpline();
	size_t End();
	vec BasisFuns(size_t, double);
	vec Values(size_t, size_t n = 25);
	std::vector<std::vector<point>> Bases();
};

BSpline::BSpline(vec knots)
{
	U = knots;
	p = 0;
	for (size_t i = 1; i < U.size(); i++)
	{
		if (U[i] == U[i - 1])
			p++;
		else
			break;
	}
}

BSpline::~BSpline()
{
}

inline size_t BSpline::End()
{
	return U.size() - p - 1;
}

inline vec BSpline::BasisFuns(size_t i, double u)
{
	auto left = [i, u, this](size_t j) {return u - U[i + 1 - j]; };
	auto right = [i, u, this](size_t j) {return U[i + j] - u; };

	vec N(p + 1);
	N[0] = 1.0;
	for (size_t j = 1; j <= p; j++)
	{
		double saved = 0.0;
		for (int k = 0; k < j; k++)
		{
			auto temp = N[k] / (right(k + 1) + left(j - k));
			N[k] = saved + right(k + 1) * temp;
			saved = left(j - k) * temp;
		}
		N[j] = saved;
	}
	return N;
}

inline vec BSpline::Values(size_t i, size_t n)
{
	if (U[i] == U[i + 1])
		return {};
	vec v(n + 1);
	v[0] = U[i];
	v[n] = U[i + 1];
	double h = (U[i + 1] - U[i]) / n;
	for (size_t j = 1; j < n; j++)
		v[j] = v[j - 1] + h;
	return v;
}

inline std::vector<std::vector<point>> BSpline::Bases()
{
	std::vector<std::vector<point>> L;
	int start = -(int)p;
	for (int i = 0; i + p + 1 < U.size(); i++)
	{
		std::vector<point> vp;
		L.push_back(vp);
		if (!Values(i).empty())
		{
			for (int j = 0; j <= p; j++)
				if (!L[start + j].empty())
					L[start + j].pop_back();
			for (auto u : Values(i))
			{
				auto v = BasisFuns(i, u);
				for (int j = 0; j <= p; j++)
				{
					point p = { u, v[j] };
					L[start + j].push_back(p);
				}
			}
		}
		start++;
	}
	return L;
}
