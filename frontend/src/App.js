// Core React and routing imports
import React from "react";
import {
  BrowserRouter as Router, // Main router component
  Routes, // Container for route definitions
  Route, // Individual route definition
  Navigate, // Programmatic navigation component
} from "react-router-dom";

// Authentication utility
import { isAuthenticated } from "./Services/auth";

// Custom components
// Authentication related components
import ProtectedRoute from "./components/ProtectedRoute"; // Wrapper for protected routes
import Login from "./components/Auth/Login"; // User login form
import Register from "./components/Auth/Register"; // User registration form
import PendingApproval from "./components/Auth/PendingApproval"; // Waiting screen for unapproved users

// Main application components
import Dashboard from "./components/Dashboard/Dashboard"; // Main intelligence dashboard
import ReportSubmission from "./components/ReportSubmission/ReportSubmission"; // Report creation form
import PersonLookup from "./components/PersonLookup/PersonLookup"; // Person search interface
import Reports from "./components/Reports/Reports"; // Reports management
import PeopleList from "./components/PeopleList"; // List of all tracked persons
import Navbar from "./components/Navbar/Navbar"; // Navigation header
import AdminDashboard from "./components/AdminDashboard"; // Administrative interface

// Style imports
import "bootstrap/dist/css/bootstrap.min.css"; // Bootstrap framework styles
import "./App.css"; // Custom application styles

/**
 * App Component
 *
 * This is the root component of the application that:
 * 1. Sets up the routing structure
 * 2. Manages authentication state
 * 3. Controls the application layout
 * 4. Handles protected and public routes
 *
 * The application uses React Router v6 with future features enabled:
 * - v7_startTransition: Enables concurrent mode features
 * - v7_relativeSplatPath: Improves path matching behavior
 */
function App() {
  return (
    <Router future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
      <div className="app-container">
        {/* 
          Conditional rendering of the navigation bar.
          Only authenticated users can see the navigation options.
          This prevents unauthorized users from accessing the navigation structure.
        */}
        {isAuthenticated() && <Navbar />}

        {/* 
          Route Definitions
          The application has three types of routes:
          1. Public routes (login, register, pending-approval)
          2. Protected routes (dashboard, submit)
          3. Feature routes (lookup, reports, people, admin)
        */}
        <Routes>
          {/* Public Routes - Accessible without authentication */}
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/pending-approval" element={<PendingApproval />} />
          {/* Protected Routes - Require authentication */}
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/submit"
            element={
              <ProtectedRoute>
                <ReportSubmission />
              </ProtectedRoute>
            }
          />
          {/* Feature Routes - Core application functionality */}
          <Route path="/lookup" element={<PersonLookup />} />{" "}
          {/* Person search and details */}
          <Route path="/reports" element={<Reports />} />{" "}
          {/* Reports management */}
          <Route path="/people" element={<PeopleList />} />{" "}
          {/* People tracking */}
          <Route path="/admin" element={<AdminDashboard />} />{" "}
          {/* Administrative controls */}
          {/* 
            Fallback Route
            Catches any undefined routes and redirects to the home page.
            The 'replace' prop ensures the redirect doesn't add to browser history.
          */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
