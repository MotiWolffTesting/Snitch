import React from "react";
import { useNavigate } from "react-router-dom";
import { logout } from "../../Services/auth";

export default function PendingApproval() {
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">
                <div className="col-md-6">
                    <div className="card shadow-lg border-0">
                        <div className="card-body p-5">
                            <div className="text-center mb-4">
                                <i
                                    className="bi bi-shield-lock text-primary"
                                    style={{ fontSize: "3rem" }}
                                ></i>
                                <h2 className="mt-3">Access Pending Approval</h2>
                            </div>

                            <div className="alert alert-warning">
                                <h5 className="alert-heading">
                                    <i className="bi bi-exclamation-triangle me-2"></i>
                                    Security Clearance Required
                                </h5>
                                <p className="mb-0">
                                    Your access request is currently under review by system
                                    administrators. This process may take up to 24 hours.
                                </p>
                            </div>

                            <div className="mt-4">
                                <h5>What happens next?</h5>
                                <ul className="list-unstyled">
                                    <li className="mb-2">
                                        <i className="bi bi-check-circle text-success me-2"></i>
                                        Your credentials will be verified
                                    </li>
                                    <li className="mb-2">
                                        <i className="bi bi-check-circle text-success me-2"></i>
                                        Security clearance will be confirmed
                                    </li>
                                    <li className="mb-2">
                                        <i className="bi bi-check-circle text-success me-2"></i>
                                        You will receive an email notification
                                    </li>
                                </ul>
                            </div>

                            <div className="d-grid gap-2 mt-4">
                                <button
                                    className="btn btn-outline-danger"
                                    onClick={handleLogout}
                                >
                                    <i className="bi bi-box-arrow-right me-2"></i>
                                    Sign Out
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
