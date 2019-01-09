#include <vtkVersion.h>
#include <vtkRenderer.h>
#include <vtkRenderWindowInteractor.h>
#include <vtkRenderWindow.h>
#include <vtkSmartPointer.h>
#include <vtkChartXY.h>
#include <vtkTable.h>
#include <vtkPlot.h>
#include <vtkDoubleArray.h>
#include <vtkContextView.h>
#include <vtkContextScene.h>
#include <vtkPen.h>
#include "BSpline.h"

typedef vtkSmartPointer<vtkTable> vtab;
typedef vtkSmartPointer<vtkDoubleArray> varr;
 
int main(int, char *[])
{
	vec knots = { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
	BSpline bs(knots);
	auto pp = bs.Bases();

	// Set up the view
	vtkSmartPointer<vtkContextView> view =
		vtkSmartPointer<vtkContextView>::New();
	view->GetRenderer()->SetBackground(1.0, 1.0, 1.0);

	vtkSmartPointer<vtkChartXY> chart =
		vtkSmartPointer<vtkChartXY>::New();
	view->GetScene()->AddItem(chart);

	for (size_t k = 0; k < pp.size(); k++)
	{
		// Create a table with some points in it
		auto table = vtab::New();
		auto X = varr::New();
		X->SetName("X");
		table->AddColumn(X);
		auto Y = varr::New();
		Y->SetName("Y");
		table->AddColumn(Y);

		
		int numPoints = pp[k].size();
		
		table->SetNumberOfRows(numPoints);
		for (int i = 0; i < numPoints; ++i)
		{
			table->SetValue(i, 0, pp[k][i][0]);
			table->SetValue(i, 1, pp[k][i][1]);
		}		

		vtkPlot *line = chart->AddPlot(vtkChart::LINE);
		line->SetInputData(table, 0, 1);
		//line->SetColor(0, 255, 0, 255);
		line->SetWidth(2.0);
		line = chart->AddPlot(vtkChart::LINE);
	}

  // Start interactor
  view->GetRenderWindow()->SetSize(1200, 600);
  view->GetInteractor()->Initialize();
  view->GetInteractor()->Start();
 
  return EXIT_SUCCESS;
}
