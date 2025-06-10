import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { logout } from "../../Services/auth";

export default function Navbar() {
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
            <div className="container">
                <Link className="navbar-brand" to="/">
                    <i className="bi bi-shield-lock me-2"></i>
                    Classified Intelligence System
                </Link>

                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarNav"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div className="collapse navbar-collapse" id="navbarNav">
                    <ul className="navbar-nav me-auto">
                        <li className="nav-item">
                            <Link className="nav-link" to="/">
                                <i className="bi bi-speedometer2 me-1"></i>
                                Dashboard
                            </Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/submit">
                                <i className="bi bi-file-earmark-text me-1"></i>
                                Submit Report
                            </Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/lookup">
                                <i className="bi bi-search me-1"></i>
                                Person Lookup
                            </Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/reports">
                                <i className="bi bi-list-ul me-1"></i>
                                Reports
                            </Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/people">
                                <i className="bi bi-people me-1"></i>
                                People
                            </Link>
                        </li>
                    </ul>

                    <div className="d-flex">
                        <button className="btn btn-outline-light" onClick={handleLogout}>
                            <i className="bi bi-box-arrow-right me-1"></i>
                            Sign Out
                        </button>
                    </div>
                </div>
            </div>
        </nav>
    );
}
