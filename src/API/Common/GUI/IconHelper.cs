using Cairo;
using System;

namespace AlchemicalRefinement.API.Common.GUI
{
    public class IconHelper
    {
        public IconHelper()
        {
            
        }
        
        public void DrawOxygen(Context cr, int x, int y, float width, float height, double[] rgba) {
            ImageSurface temp_surface;
            Context old_cr;
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;
	
            cr.Save();
            float w = 16;
            float h = 16;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.6;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Round; // maybe needs to be 0
            cr.LineJoin = LineJoin.Round; // maybe needs to be 0
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);
	
            cr.NewPath();
            cr.MoveTo(12, 8);
            cr.CurveTo(12, 5.792969, 10.207031, 4, 8, 4);
            cr.CurveTo(5.792969, 4, 4, 5.792969, 4, 8);
            cr.CurveTo(4, 10.207031, 5.792969, 12, 8, 12);
            cr.CurveTo(10.207031, 12, 12, 10.207031, 12, 8);
            cr.ClosePath();
            cr.MoveTo(1.332031, 8);
            cr.LineTo(14.667969, 8);
            cr.MoveTo(14.667969, 6.820313);
            cr.LineTo(14.667969, 9.179688);
            cr.MoveTo(8, 1.332031);
            cr.LineTo(8, 14.667969);
            cr.MoveTo(1.332031, 6.820313);
            cr.LineTo(1.332031, 9.179688);
            cr.MoveTo(9.179688, 1.332031);
            cr.LineTo(6.820313, 1.332031);
            cr.MoveTo(9.179688, 14.667969);
            cr.LineTo(6.820313, 14.667969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1.333333,0,0,1.333333,0,0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }
        
        public void DrawSulphur(Context cr, int x, int y, float width, float height, double[] rgba) {
            ImageSurface temp_surface;
            Context old_cr;
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;
	
            cr.Save();
            float w = 819;
            float h = 1000;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);
	
            cr.NewPath();
            cr.MoveTo(786, 749);
            cr.LineTo(721, 684);
            cr.LineTo(674, 684);
            cr.LineTo(579, 779);
            cr.LineTo(483, 684);
            cr.LineTo(405, 684);
            cr.LineTo(450, 801);
            cr.LineTo(323, 801);
            cr.LineTo(370, 684);
            cr.LineTo(261, 684);
            cr.LineTo(235, 708);
            cr.LineTo(191, 663);
            cr.LineTo(235, 619);
            cr.LineTo(259, 643);
            cr.LineTo(370, 643);
            cr.LineTo(370, 577);
            cr.LineTo(41, 577);
            cr.LineTo(419, 114);
            cr.LineTo(786, 577);
            cr.LineTo(407, 577);
            cr.LineTo(407, 643);
            cr.LineTo(494, 643);
            cr.LineTo(579, 728);
            cr.LineTo(663, 643);
            cr.LineTo(736, 643);
            cr.LineTo(786, 593);
            cr.ClosePath();
            cr.MoveTo(695, 533);
            cr.LineTo(419, 185);
            cr.LineTo(140, 533);
            cr.ClosePath();
            cr.MoveTo(695, 533);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }
        
        public void DrawCalcination(Context cr, int x, int y, float width, float height, double[] rgba) {
            ImageSurface temp_surface;
			Context old_cr;
			Pattern pattern = null;
			Matrix matrix = cr.Matrix;
	
			cr.Save();
			float w = 2058;
			float h = 2048;
			float scale = Math.Min(width / w, height / h);
			matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
			matrix.Scale(scale, scale);
			cr.Matrix = matrix;

			cr.Operator = Operator.Over;
			pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
			cr.SetSource(pattern);
	
			cr.NewPath();
			cr.MoveTo(244, 438);
			cr.LineTo(381, 457);
			cr.LineTo(537, 447);
			cr.LineTo(576, 457);
			cr.LineTo(586, 447);
			cr.LineTo(634, 457);
			cr.LineTo(790, 438);
			cr.CurveTo(822.667969, 442.667969, 839, 452, 839, 466);
			cr.LineTo(556, 912);
			cr.LineTo(410, 1191);
			cr.LineTo(410, 1210);
			cr.CurveTo(476.667969, 1289.332031, 597, 1370, 771, 1452);
			cr.LineTo(1073, 1554);
			cr.LineTo(1083, 1564);
			cr.LineTo(1083, 1573);
			cr.CurveTo(1077.667969, 1616.332031, 1058, 1638, 1024, 1638);
			cr.CurveTo(1024, 1622.667969, 943, 1594.667969, 781, 1554);
			cr.LineTo(586, 1489);
			cr.LineTo(390, 1378);
			cr.CurveTo(390, 1366, 354.332031, 1328.667969, 283, 1266);
			cr.LineTo(186, 1071);
			cr.LineTo(195, 1061);
			cr.LineTo(195, 1052);
			cr.CurveTo(137, 944.667969, 88.332031, 799, 49, 615);
			cr.LineTo(39, 615);
			cr.LineTo(49, 587);
			cr.LineTo(10, 466);
			cr.LineTo(49, 447);
			cr.LineTo(147, 466);
			cr.LineTo(156, 457);
			cr.LineTo(166, 457);
			cr.LineTo(205, 466);
			cr.ClosePath();
			cr.MoveTo(137, 578);
			cr.LineTo(137, 605);
			cr.LineTo(156, 624);
			cr.LineTo(147, 633);
			cr.LineTo(147, 643);
			cr.CurveTo(221.667969, 914.332031, 273.667969, 1060, 303, 1080);
			cr.LineTo(332, 1108);
			cr.LineTo(361, 1108);
			cr.CurveTo(361, 1089.332031, 383.667969, 1052, 429, 996);
			cr.CurveTo(429, 962.667969, 478, 863.332031, 576, 698);
			cr.CurveTo(576, 682.667969, 595.332031, 651.667969, 634, 605);
			cr.LineTo(586, 568);
			cr.LineTo(576, 568);
			cr.LineTo(429, 578);
			cr.LineTo(429, 568);
			cr.LineTo(420, 578);
			cr.LineTo(322, 568);
			cr.LineTo(254, 568);
			cr.ClosePath();
			cr.MoveTo(137, 578);
			cr.Tolerance = 0.1;
			cr.Antialias = Antialias.Default;
			cr.FillRule = FillRule.Winding;
			cr.FillPreserve();
	
			if (pattern!= null) pattern.Dispose();

			cr.Restore();
        }
        
        public void DrawFireCubed(Context cr, int x, int y, float width, float height, double[] rgba) {
	        ImageSurface temp_surface;
	        Context old_cr;
	        Pattern pattern = null;
	        Matrix matrix = cr.Matrix;
	        
	        cr.Save();
	        float w = 80;
	        float h = 80;
	        float scale = Math.Min(width / w, height / h);
	        matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
	        matrix.Scale(scale, scale);
	        cr.Matrix = matrix;
	
	        cr.Operator = Operator.Over;
	        pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
	        cr.SetSource(pattern);
	
	        cr.NewPath();
	        cr.MoveTo(49.414063, 0.117188);
			cr.LineTo(49.335938, 0.695313);
	        cr.CurveTo(48.542969, 7.339844, 49.980469, 11.796875, 55.089844, 17.402344);
			cr.CurveTo(57.671875, 20.246094, 59.882813, 23, 60.613281, 26.433594);
			cr.LineTo(61.390625, 25.9375);
			cr.CurveTo(63.417969, 24.324219, 65.34375, 23.640625, 68.179688, 23.277344);
			cr.CurveTo(70.375, 22.898438, 70.386719, 22.902344, 71.039063, 24.59375);
			cr.CurveTo(72.019531, 25.847656, 72.527344, 27.199219, 73.359375, 28.511719);
			cr.CurveTo(76.921875, 34.195313, 79.878906, 40.367188, 79.945313, 46.699219);
			cr.CurveTo(79.960938, 48.128906, 79.960938, 49.5625, 79.984375, 50.996094);
			cr.CurveTo(80.046875, 54.355469, 79.992188, 57.378906, 78.460938, 60.566406);
			cr.LineTo(78.195313, 61.171875);
			cr.CurveTo(76.355469, 65.1875, 73.480469, 69.355469, 69.011719, 72.03125);
			cr.CurveTo(65.757813, 73.898438, 62.515625, 75.851563, 58.855469, 77.265625);
			cr.CurveTo(56.5625, 78.089844, 54.136719, 78.65625, 51.742188, 79.277344);
			cr.CurveTo(48.351563, 80.046875, 44.890625, 79.984375, 41.382813, 79.976563);
			cr.CurveTo(40.300781, 79.976563, 39.222656, 79.988281, 38.140625, 80);
			cr.CurveTo(31.398438, 80.03125, 25.695313, 78.910156, 19.664063, 76.71875);
			cr.CurveTo(16.621094, 75.761719, 14.160156, 74.148438, 11.597656, 72.652344);
			cr.CurveTo(5.421875, 68.929688, 1.390625, 62.65625, 0.0664063, 56.917969);
			cr.CurveTo(-0.00390625, 56.121094, 0, 55.324219, 0, 54.523438);
			cr.LineTo(0.0195313, 53.585938);
			cr.CurveTo(1.527344, 53.386719, 1.542969, 53.398438, 4.265625, 53.269531);
			cr.LineTo(4.964844, 53.269531);
			cr.LineTo(4.671875, 52.414063);
			cr.CurveTo(4.359375, 51.496094, 4.046875, 50.578125, 3.734375, 49.660156);
			cr.CurveTo(2.476563, 45.472656, 2.636719, 40.453125, 4.265625, 36.335938);
			cr.LineTo(4.625, 35.378906);
			cr.CurveTo(6.09375, 31.8125, 8.585938, 26.667969, 13.015625, 24.609375);
			cr.CurveTo(14.066406, 24.5625, 14.066406, 24.5625, 15.113281, 24.871094);
			cr.CurveTo(15.941406, 25.851563, 16.445313, 26.769531, 17.863281, 29.394531);
			cr.CurveTo(18.253906, 30.121094, 18.253906, 30.121094, 18.964844, 30.863281);
			cr.LineTo(19.058594, 30.257813);
			cr.CurveTo(20.722656, 19.90625, 24.621094, 10.480469, 36.070313, 3.734375);
			cr.CurveTo(38.34375, 2.425781, 39.5, 1.886719, 41.164063, 0.933594);
			cr.CurveTo(43.597656, -0.3125, 46.589844, 0.0117188, 49.414063, 0.117188);
			cr.ClosePath();
			cr.MoveTo(35.761719, 7.753906);
			cr.CurveTo(28.152344, 12.726563, 23.886719, 20.578125, 23.164063, 27.996094);
			cr.CurveTo(23.082031, 29.253906, 22.996094, 31.207031, 22.878906, 34.128906);
			cr.CurveTo(22.8125, 34.773438, 22.8125, 34.773438, 22.464844, 35.292969);
			cr.CurveTo(20.035156, 35.210938, 18.484375, 34.921875, 16.695313, 33.605469);
			cr.CurveTo(15.199219, 32.386719, 14.105469, 30.992188, 13.015625, 29.5625);
			cr.CurveTo(9.097656, 32.25, 7.320313, 36.859375, 7.003906, 40.734375);
			cr.CurveTo(6.878906, 45.320313, 7.101563, 49.242188, 9.601563, 53.472656);
			cr.CurveTo(10.050781, 54.460938, 9.953125, 55.363281, 9.863281, 56.398438);
			cr.CurveTo(8.660156, 56.453125, 6.804688, 56.625, 4.617188, 56.65625);
			cr.CurveTo(4.640625, 61.652344, 8.367188, 66.21875, 13.015625, 69.683594);
			cr.CurveTo(15.257813, 71.1875, 17.667969, 72.574219, 20.363281, 73.59375);
			cr.LineTo(21.0625, 73.332031);
			cr.LineTo(20.277344, 72.503906);
			cr.CurveTo(17.09375, 68.726563, 16.875, 63.761719, 18.132813, 59.507813);
			cr.CurveTo(18.902344, 56.960938, 20.257813, 54.550781, 21.527344, 52.128906);
			cr.CurveTo(22.359375, 50.574219, 23.300781, 49.183594, 24.5625, 47.800781);
			cr.CurveTo(26.871094, 45.101563, 29.242188, 42.703125, 33.097656, 39.230469);
			cr.CurveTo(34.261719, 37.820313, 35.6875, 36.621094, 37.257813, 35.441406);
			cr.CurveTo(38.253906, 34.742188, 38.777344, 34.511719, 40.164063, 34.492188);
			cr.CurveTo(42.140625, 34.953125, 43.222656, 35.84375, 44.492188, 37.019531);
			cr.CurveTo(45.28125, 37.75, 46.058594, 38.488281, 46.839844, 39.226563);
			cr.CurveTo(50.207031, 42.410156, 49.191406, 41.496094, 51.976563, 44.144531);
			cr.CurveTo(58.207031, 50.242188, 63.261719, 58.5, 62.363281, 66.296875);
			cr.CurveTo(61.828125, 68.777344, 60.761719, 71.101563, 59.210938, 73.332031);
			cr.CurveTo(65.132813, 72.054688, 69.085938, 67.890625, 72.152344, 64.152344);
			cr.CurveTo(78.28125, 55.753906, 76.917969, 46.15625, 73.503906, 37.328125);
			cr.CurveTo(72.058594, 33.722656, 70.214844, 30.320313, 67.960938, 26.953125);
			cr.CurveTo(66.011719, 26.953125, 65.144531, 27.539063, 63.761719, 28.519531);
			cr.CurveTo(62.316406, 29.796875, 61.304688, 31.34375, 59.5625, 32.425781);
			cr.CurveTo(58.597656, 32.492188, 58.597656, 32.492188, 57.8125, 32.425781);
			cr.LineTo(57.667969, 31.460938);
			cr.CurveTo(56.707031, 25.617188, 53.773438, 20.648438, 49.105469, 15.898438);
			cr.CurveTo(45.574219, 12.269531, 45.027344, 7.828125, 44.511719, 3.503906);
			cr.CurveTo(40.816406, 3.695313, 38.445313, 6.109375, 35.761719, 7.753906);
			cr.ClosePath();
			cr.MoveTo(38.519531, 38.371094);
			cr.CurveTo(35.929688, 40.558594, 33.539063, 42.847656, 31.214844, 45.195313);
			cr.LineTo(30.753906, 45.65625);
			cr.CurveTo(25.480469, 51.023438, 21.46875, 57, 20.714844, 63.691406);
			cr.LineTo(20.625, 64.398438);
			cr.CurveTo(20.429688, 67.300781, 22.050781, 69.941406, 24.214844, 72.289063);
			cr.LineTo(24.828125, 72.992188);
			cr.CurveTo(28.066406, 75.46875, 32.933594, 77.164063, 37.59375, 77.320313);
			cr.CurveTo(38.21875, 77.324219, 38.308594, 77.328125, 39.960938, 77.320313);
			cr.CurveTo(45.621094, 77.238281, 50.554688, 76.046875, 54.640625, 73.105469);
			cr.CurveTo(57.628906, 70.726563, 59.148438, 67.65625, 59.34375, 64.410156);
			cr.CurveTo(58.847656, 55.652344, 52.265625, 48.277344, 44.980469, 41.761719);
			cr.CurveTo(44.234375, 41.089844, 43.035156, 39.972656, 41.453125, 38.480469);
			cr.CurveTo(40.386719, 37.695313, 39.683594, 37.632813, 38.519531, 38.371094);
			cr.ClosePath();
			cr.MoveTo(38.519531, 38.371094);
			cr.Tolerance = 0.1;
			cr.Antialias = Antialias.Default;
			cr.FillRule = FillRule.Winding;
			cr.FillPreserve();
			if (pattern != null) pattern.Dispose();

			cr.Restore();
        }

        public void DrawFireRectangle(Context cr, int x, int y, float width, float height, double[] rgba) {
	
	        ImageSurface temp_surface;
	        Context old_cr;
	        Pattern pattern = null;
	        Matrix matrix = cr.Matrix;
	
	        cr.Save();
	        float w = 60;
	        float h = 80;
	        float scale = Math.Min(width / w, height / h);
	        matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
	        matrix.Scale(scale, scale);
	        cr.Matrix = matrix;

	        cr.Operator = Operator.Over;
	        pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
	        cr.SetSource(pattern);
	
	
	        cr.NewPath();
	        cr.MoveTo(37.058594, 0.117188);
	        cr.LineTo(37, 0.695313);
	        cr.CurveTo(36.40625, 7.339844, 37.484375, 11.796875, 41.316406, 17.402344);
	        cr.CurveTo(43.253906, 20.246094, 44.914063, 23, 45.457031, 26.433594);
	        cr.LineTo(46.042969, 25.9375);
	        cr.CurveTo(47.5625, 24.324219, 49.007813, 23.640625, 51.136719, 23.277344);
	        cr.CurveTo(52.78125, 22.898438, 52.789063, 22.902344, 53.277344, 24.59375);
	        cr.CurveTo(54.015625, 25.847656, 54.394531, 27.199219, 55.019531, 28.511719);
	        cr.CurveTo(57.691406, 34.195313, 59.910156, 40.367188, 59.957031, 46.699219);
	        cr.CurveTo(59.96875, 48.128906, 59.972656, 49.5625, 59.988281, 50.996094);
	        cr.CurveTo(60.035156, 54.355469, 59.996094, 57.378906, 58.84375, 60.566406);
	        cr.LineTo(58.644531, 61.171875);
	        cr.CurveTo(57.265625, 65.1875, 55.109375, 69.355469, 51.757813, 72.03125);
	        cr.CurveTo(49.316406, 73.898438, 46.886719, 75.851563, 44.140625, 77.265625);
	        cr.CurveTo(42.421875, 78.089844, 40.601563, 78.65625, 38.804688, 79.277344);
	        cr.CurveTo(36.261719, 80.046875, 33.667969, 79.984375, 31.039063, 79.976563);
	        cr.CurveTo(30.226563, 79.976563, 29.414063, 79.988281, 28.605469, 80);
	        cr.CurveTo(23.550781, 80.03125, 19.273438, 78.910156, 14.75, 76.71875);
	        cr.CurveTo(12.464844, 75.761719, 10.621094, 74.148438, 8.699219, 72.652344);
	        cr.CurveTo(4.066406, 68.929688, 1.042969, 62.65625, 0.0507813, 56.917969);
	        cr.CurveTo(-0.00390625, 56.121094, 0, 55.324219, 0, 54.523438);
	        cr.LineTo(0.015625, 53.585938);
	        cr.CurveTo(1.148438, 53.386719, 1.15625, 53.398438, 3.199219, 53.269531);
	        cr.LineTo(3.722656, 53.269531);
	        cr.LineTo(3.503906, 52.414063);
	        cr.CurveTo(3.269531, 51.496094, 3.035156, 50.578125, 2.800781, 49.660156);
	        cr.CurveTo(1.855469, 45.472656, 1.976563, 40.453125, 3.199219, 36.335938);
	        cr.LineTo(3.46875, 35.378906);
	        cr.CurveTo(4.570313, 31.8125, 6.4375, 26.667969, 9.761719, 24.609375);
	        cr.CurveTo(10.546875, 24.5625, 10.546875, 24.5625, 11.335938, 24.871094);
	        cr.CurveTo(11.957031, 25.851563, 12.335938, 26.769531, 13.398438, 29.394531);
	        cr.CurveTo(13.691406, 30.121094, 13.691406, 30.121094, 14.222656, 30.863281);
	        cr.LineTo(14.292969, 30.257813);
	        cr.CurveTo(15.542969, 19.90625, 18.464844, 10.480469, 27.050781, 3.734375);
	        cr.CurveTo(28.757813, 2.425781, 29.625, 1.886719, 30.871094, 0.933594);
	        cr.CurveTo(32.699219, -0.3125, 34.941406, 0.0117188, 37.058594, 0.117188);
	        cr.ClosePath();
	        cr.MoveTo(26.824219, 7.753906);
	        cr.CurveTo(21.113281, 12.726563, 17.914063, 20.578125, 17.375, 27.996094);
	        cr.CurveTo(17.3125, 29.253906, 17.246094, 31.207031, 17.160156, 34.128906);
	        cr.CurveTo(17.109375, 34.773438, 17.109375, 34.773438, 16.847656, 35.292969);
	        cr.CurveTo(15.027344, 35.210938, 13.863281, 34.921875, 12.519531, 33.605469);
	        cr.CurveTo(11.398438, 32.386719, 10.578125, 30.992188, 9.761719, 29.5625);
	        cr.CurveTo(6.824219, 32.25, 5.492188, 36.859375, 5.253906, 40.734375);
	        cr.CurveTo(5.160156, 45.320313, 5.328125, 49.242188, 7.203125, 53.472656);
	        cr.CurveTo(7.535156, 54.460938, 7.464844, 55.363281, 7.398438, 56.398438);
	        cr.CurveTo(6.496094, 56.453125, 5.105469, 56.625, 3.460938, 56.65625);
	        cr.CurveTo(3.480469, 61.652344, 6.277344, 66.21875, 9.761719, 69.683594);
	        cr.CurveTo(11.441406, 71.1875, 13.25, 72.574219, 15.273438, 73.59375);
	        cr.LineTo(15.796875, 73.332031);
	        cr.LineTo(15.207031, 72.503906);
	        cr.CurveTo(12.820313, 68.726563, 12.65625, 63.761719, 13.601563, 59.507813);
	        cr.CurveTo(14.175781, 56.960938, 15.191406, 54.550781, 16.144531, 52.128906);
	        cr.CurveTo(16.769531, 50.574219, 17.476563, 49.183594, 18.421875, 47.800781);
	        cr.CurveTo(20.152344, 45.101563, 21.933594, 42.703125, 24.820313, 39.230469);
	        cr.CurveTo(25.695313, 37.820313, 26.765625, 36.621094, 27.941406, 35.441406);
	        cr.CurveTo(28.691406, 34.742188, 29.082031, 34.511719, 30.125, 34.492188);
	        cr.CurveTo(31.605469, 34.953125, 32.417969, 35.84375, 33.367188, 37.019531);
	        cr.CurveTo(33.960938, 37.75, 34.542969, 38.488281, 35.128906, 39.226563);
	        cr.CurveTo(37.652344, 42.410156, 36.894531, 41.496094, 38.980469, 44.144531);
	        cr.CurveTo(43.65625, 50.242188, 47.445313, 58.5, 46.769531, 66.296875);
	        cr.CurveTo(46.371094, 68.777344, 45.570313, 71.101563, 44.410156, 73.332031);
	        cr.CurveTo(48.851563, 72.054688, 51.816406, 67.890625, 54.117188, 64.152344);
	        cr.CurveTo(58.710938, 55.753906, 57.6875, 46.15625, 55.128906, 37.328125);
	        cr.CurveTo(54.046875, 33.722656, 52.660156, 30.320313, 50.972656, 26.953125);
	        cr.CurveTo(49.507813, 26.953125, 48.859375, 27.539063, 47.820313, 28.519531);
	        cr.CurveTo(46.738281, 29.796875, 45.976563, 31.34375, 44.671875, 32.425781);
	        cr.CurveTo(43.949219, 32.492188, 43.949219, 32.492188, 43.359375, 32.425781);
	        cr.LineTo(43.25, 31.460938);
	        cr.CurveTo(42.53125, 25.617188, 40.328125, 20.648438, 36.828125, 15.898438);
	        cr.CurveTo(34.179688, 12.269531, 33.773438, 7.828125, 33.382813, 3.503906);
	        cr.CurveTo(30.609375, 3.695313, 28.832031, 6.109375, 26.824219, 7.753906);
	        cr.ClosePath();
	        cr.MoveTo(28.890625, 38.371094);
	        cr.CurveTo(26.949219, 40.558594, 25.152344, 42.847656, 23.410156, 45.195313);
	        cr.LineTo(23.066406, 45.65625);
	        cr.CurveTo(19.113281, 51.023438, 16.101563, 57, 15.535156, 63.691406);
	        cr.LineTo(15.46875, 64.398438);
	        cr.CurveTo(15.324219, 67.300781, 16.539063, 69.941406, 18.160156, 72.289063);
	        cr.LineTo(18.621094, 72.992188);
	        cr.CurveTo(21.050781, 75.46875, 24.699219, 77.164063, 28.195313, 77.320313);
	        cr.CurveTo(28.664063, 77.324219, 28.734375, 77.328125, 29.96875, 77.320313);
	        cr.CurveTo(34.214844, 77.238281, 37.917969, 76.046875, 40.980469, 73.105469);
	        cr.CurveTo(43.222656, 70.726563, 44.363281, 67.65625, 44.507813, 64.410156);
	        cr.CurveTo(44.136719, 55.652344, 39.199219, 48.277344, 33.734375, 41.761719);
	        cr.CurveTo(33.175781, 41.089844, 32.277344, 39.972656, 31.089844, 38.480469);
	        cr.CurveTo(30.289063, 37.695313, 29.761719, 37.632813, 28.890625, 38.371094);
	        cr.ClosePath();
	        cr.MoveTo(28.890625, 38.371094);
	        cr.Tolerance = 0.1;
	        cr.Antialias = Antialias.Default;
	        cr.FillRule = FillRule.Winding;
	        cr.FillPreserve();
	        if (pattern != null) pattern.Dispose();

	        cr.Restore();
        }
        
        public void DrawMercury(Context cr, int x, int y, float width, float height, double[] rgba) {
	
	        ImageSurface temp_surface;
	        Context old_cr;
	        Pattern pattern = null;
	        Matrix matrix = cr.Matrix;
	
	        cr.Save();
	        float w = 408;
	        float h = 1000;
	        float scale = Math.Min(width / w, height / h);
	        matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
	        matrix.Scale(scale, scale);
	        cr.Matrix = matrix;

	
	        cr.Operator = Operator.Over;
	        pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
	        cr.SetSource(pattern);
	
	        cr.NewPath();
	        cr.MoveTo(388, 68);
	        cr.CurveTo(385.332031, 108.667969, 354.667969, 148.667969, 296, 188);
	        cr.CurveTo(346.667969, 238, 372, 286, 372, 332);
	        cr.CurveTo(372, 353.332031, 360, 384.332031, 336, 425);
	        cr.LineTo(334, 423);
	        cr.CurveTo(306.667969, 460.332031, 273, 481, 233, 485);
	        cr.LineTo(234, 598);
	        cr.LineTo(307, 598);
	        cr.LineTo(337, 568);
	        cr.LineTo(386, 617);
	        cr.LineTo(335, 669);
	        cr.LineTo(301, 645);
	        cr.LineTo(301, 644);
	        cr.LineTo(232, 644);
	        cr.LineTo(233, 774);
	        cr.LineTo(262, 797);
	        cr.LineTo(210, 848);
	        cr.LineTo(160, 799);
	        cr.LineTo(184, 779);
	        cr.LineTo(184, 644);
	        cr.LineTo(112, 644);
	        cr.LineTo(83, 676);
	        cr.LineTo(31, 626);
	        cr.LineTo(82, 575);
	        cr.LineTo(103, 598);
	        cr.LineTo(181, 598);
	        cr.LineTo(181, 483);
	        cr.CurveTo(135.667969, 477.667969, 101, 459.667969, 77, 429);
	        cr.CurveTo(53.667969, 400.332031, 42, 363, 42, 317);
	        cr.CurveTo(42, 278.332031, 71.667969, 234.332031, 131, 185);
	        cr.CurveTo(112.332031, 169.667969, 95, 151, 79, 129);
	        cr.CurveTo(59, 101.667969, 48.667969, 79.332031, 48, 62);
	        cr.CurveTo(72.667969, 82, 97.332031, 102.332031, 122, 123);
	        cr.CurveTo(153.332031, 147, 182.332031, 159, 209, 159);
	        cr.CurveTo(278.332031, 159, 338, 128.667969, 388, 68);
	        cr.ClosePath();
	        cr.MoveTo(322, 324);
	        cr.CurveTo(322, 299.332031, 309.667969, 274.832031, 285, 250.5);
	        cr.CurveTo(260.332031, 226.167969, 235.332031, 214, 210, 214);
	        cr.CurveTo(179.332031, 214, 152, 225.667969, 128, 249);
	        cr.CurveTo(104, 272.332031, 92, 299.332031, 92, 330);
	        cr.CurveTo(92, 364, 103.332031, 390.332031, 126, 409);
	        cr.CurveTo(148.667969, 427.667969, 177.332031, 437, 212, 437);
	        cr.CurveTo(237.332031, 437, 262, 424.667969, 286, 400);
	        cr.CurveTo(310, 375.332031, 322, 350, 322, 324);
	        cr.ClosePath();
	        cr.MoveTo(322, 324);
	        cr.Tolerance = 0.1;
	        cr.Antialias = Antialias.Default;
	        cr.FillRule = FillRule.Winding;
	        cr.FillPreserve();
	        if (pattern != null) pattern.Dispose();

	        cr.Restore();
        }
        

        
    }
}