import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { isAuthenticated, isApproved } from "../Services/auth";

export default function ProtectedRoute({ children, requireApproval = true }) {
    const location = useLocation();

    if (!isAuthenticated()) {
        // Redirect to login if not authenticated
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    if (requireApproval && !isApproved()) {
        // Redirect to pending approval page if not approved
        return (
            <Navigate to="/pending-approval" state={{ from: location }} replace />
        );
    }

    return children;
}
