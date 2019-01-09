
#include <iostream>
#include "mex.hpp""
#include "mexAdapter.hpp"

using namespace matlab::data;
using matlab::mex::ArgumentList;
#include "bspline.h"

class MexFunction : public matlab::mex::Function 
{
public:
    void operator()(ArgumentList outputs, ArgumentList inputs) {        
        vec knots;    
        for(size_t i = 0; i < inputs.size(); i++)
        {
            knots.push_back(inputs[i][0]);
        }
        BSpline bs(knots);
	    auto pp = bs.Bases();

        ArrayFactory factory;
        size_t np=pp.size();
        auto X=factory.createCellArray({1,np});
        auto Y=factory.createCellArray({1,np});
        
        for(size_t i = 0; i < np; i++)
        {
            size_t n=pp[i].size();
            auto x=factory.createArray<double>({1,n});
            auto y=factory.createArray<double>({1,n});
            for(size_t j = 0; j < n; j++)
            {
                x[j]=pp[i][j][0];
                y[j]=pp[i][j][1];
            }
            X[i]=x;
            Y[i]=y;
        }
        
        outputs[0] = X;
        outputs[1] = Y;
    }   
};
