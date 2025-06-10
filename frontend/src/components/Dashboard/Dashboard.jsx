import React, { useEffect, useState } from "react";
import { getPeople, getAlerts } from "../../Services/api";
import "bootstrap/dist/css/bootstrap.min.css";

/**
 * Dashboard Component
 * Main intelligence dashboard displaying key metrics, quality informants,
 * high-risk targets, and active alerts.
 */
export default function Dashboard() {
    // State management for people data, alerts, loading state, and error handling
    const [people, setPeople] = useState([]);
    const [alerts, setAlerts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Fetch data on component mount
    useEffect(() => {
        const fetchData = async () => {
            try {
                // Fetch both people and alerts data concurrently
                const [peopleData, alertsData] = await Promise.all([
                    getPeople(),
                    getAlerts(),
                ]);
                setPeople(peopleData);
                setAlerts(alertsData);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    // Filter and categorize data for display
    const stars = people.filter((p) => p.recruitScore > 0.7); // Quality informants with high recruit scores
    const dangerous = people.filter(
        (p) => p.riskLevel === "HIGH" || p.riskLevel === "CRITICAL"
    ); // High-risk targets
    const activeAlerts = alerts.filter((a) => a.status !== "RESOLVED"); // Non-resolved alerts

    return (
        <div className="container mt-4">
            <h2 className="mb-4">Malshinon Intelligence Dashboard</h2>

            {/* Key Metrics Section - Displaying summary cards */}
            <div className="row mb-4">
                {/* Total People Card */}
                <div className="col-md-3">
                    <div className="card text-bg-primary">
                        <div className="card-body text-center">
                            <h3 className="card-title">{people.length}</h3>
                            <p className="card-text">Total People</p>
                        </div>
                    </div>
                </div>
                {/* Quality Informants Card */}
                <div className="col-md-3">
                    <div className="card text-bg-success">
                        <div className="card-body text-center">
                            <h3 className="card-title">{stars.length}</h3>
                            <p className="card-text">Quality Informants</p>
                        </div>
                    </div>
                </div>
                {/* High Risk Targets Card */}
                <div className="col-md-3">
                    <div className="card text-bg-danger">
                        <div className="card-body text-center">
                            <h3 className="card-title">{dangerous.length}</h3>
                            <p className="card-text">High Risk Targets</p>
                        </div>
                    </div>
                </div>
                {/* Active Alerts Card */}
                <div className="col-md-3">
                    <div className="card text-bg-warning">
                        <div className="card-body text-center">
                            <h3 className="card-title">{activeAlerts.length}</h3>
                            <p className="card-text">Active Alerts</p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Quality Informants Section - Displaying high-scoring informants */}
            <div className="card mb-4">
                <div className="card-header bg-success text-white">
                    <h4 className="mb-0">Quality Informants</h4>
                </div>
                <div className="card-body">
                    {loading ? (
                        <div className="alert alert-info">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : (
                        <div className="list-group">
                            {stars.map((person) => (
                                <div key={person.id} className="list-group-item">
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h5 className="mb-1">{person.name}</h5>
                                            <small className="text-muted">
                                                Code: {person.secretCode}
                                            </small>
                                        </div>
                                        <div>
                                            <span className="badge bg-success">
                                                Score: {person.recruitScore.toFixed(2)}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            {/* High Risk Targets Section - Displaying dangerous individuals */}
            <div className="card mb-4">
                <div className="card-header bg-danger text-white">
                    <h4 className="mb-0">High Risk Targets</h4>
                </div>
                <div className="card-body">
                    {loading ? (
                        <div className="alert alert-info">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : (
                        <div className="list-group">
                            {dangerous.map((person) => (
                                <div key={person.id} className="list-group-item">
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h5 className="mb-1">{person.name}</h5>
                                            <small className="text-muted">
                                                Code: {person.secretCode}
                                            </small>
                                        </div>
                                        <div>
                                            <span
                                                className={`badge bg-${person.riskLevel === "CRITICAL" ? "danger" : "warning"
                                                    }`}
                                            >
                                                {person.riskLevel}
                                            </span>
                                            <span className="ms-2 badge bg-secondary">
                                                Threat: {person.threatScore.toFixed(2)}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            {/* Active Alerts Section - Displaying current alerts */}
            <div className="card">
                <div className="card-header bg-warning">
                    <h4 className="mb-0">Active Alerts</h4>
                </div>
                <div className="card-body">
                    {loading ? (
                        <div className="alert alert-info">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : (
                        <div className="list-group">
                            {activeAlerts.map((alert) => (
                                <div key={alert.id} className="list-group-item">
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h5 className="mb-1">{alert.title}</h5>
                                            <p className="mb-1">{alert.description}</p>
                                            <small className="text-muted">
                                                Created: {new Date(alert.createdAt).toLocaleString()}
                                            </small>
                                        </div>
                                        <span
                                            className={`badge bg-${alert.severity.toLowerCase()}`}
                                        >
                                            {alert.severity}
                                        </span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
