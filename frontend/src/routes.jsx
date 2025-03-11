import { createBrowserRouter } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Home from "./pages/Home";
import Hotel from "./pages/Hotel/Hotel";
import ProtectedRoute from "./protectedRoute";
import Booking from "./pages/Booking/Booking";

const routes = createBrowserRouter([
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <Home/>
      </ProtectedRoute>
    )
  },
  {
    path: "/login",
    element: <Login/>,
  },
  {
    path: "/register",
    element: <Register/>
  },
  {
    path: "/hotel",
    element: (
      <ProtectedRoute>
        <Hotel/>
      </ProtectedRoute>
    )
  },
  {
    path: "/booking",
    element: (
      <ProtectedRoute>
        <Booking/>
      </ProtectedRoute>
    )
  }
]);

export default routes;