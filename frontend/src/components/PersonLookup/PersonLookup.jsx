import React, { useState } from "react";
import { getPeople, getPerson } from "../../Services/api";

const roleLabels = {
    0: "Agent",
    1: "Terrorist",
    2: "Intel",
    3: "Civilian",
};

export default function PersonLookup() {
    const [search, setSearch] = useState("");
    const [roleFilter, setRoleFilter] = useState("");
    const [results, setResults] = useState([]);
    const [selected, setSelected] = useState(null);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleSearch = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setSelected(null);
        try {
            const people = await getPeople();
            setResults(
                people.filter((p) => {
                    const matchesName = p.name
                        .toLowerCase()
                        .includes(search.toLowerCase());
                    const matchesRole =
                        roleFilter === "" || String(p.role) === roleFilter;
                    return matchesName && matchesRole;
                })
            );
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleSelect = async (id) => {
        setLoading(true);
        setError(null);
        try {
            const person = await getPerson(id);
            setSelected(person);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="mb-4">Lookup Individuals</h2>
            <form className="row g-3 mb-3" onSubmit={handleSearch}>
                <div className="col-md-5">
                    <input
                        className="form-control"
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        placeholder="Enter name..."
                    />
                </div>
                <div className="col-md-4">
                    <select
                        className="form-select"
                        value={roleFilter}
                        onChange={(e) => setRoleFilter(e.target.value)}
                    >
                        <option value="">All Roles</option>
                        <option value="0">Agent</option>
                        <option value="1">Terrorist</option>
                        <option value="2">Intel</option>
                        <option value="3">Civilian</option>
                    </select>
                </div>
                <div className="col-md-3">
                    <button
                        type="submit"
                        className="btn btn-primary w-100"
                        disabled={loading}
                    >
                        Search
                    </button>
                </div>
            </form>
            {error && <div className="alert alert-danger">{error}</div>}
            <ul className="list-group mb-3">
                {results.map((p) => (
                    <li
                        key={p.id}
                        className="list-group-item d-flex justify-content-between align-items-center"
                    >
                        <span>
                            <span className="fw-bold">{p.name}</span>{" "}
                            <span className="badge bg-secondary ms-2">
                                {roleLabels[p.role]}
                            </span>{" "}
                            <span className="text-muted ms-2">(ID: {p.id})</span>
                        </span>
                        <button
                            className="btn btn-outline-secondary btn-sm"
                            onClick={() => handleSelect(p.id)}
                        >
                            View
                        </button>
                    </li>
                ))}
            </ul>
            {selected && (
                <div className="card mt-4">
                    <div className="card-header">{roleLabels[selected.role]} Details</div>
                    <div className="card-body">
                        <div>
                            <b>Name:</b> {selected.name}
                        </div>
                        <div>
                            <b>Role:</b> {roleLabels[selected.role]}
                        </div>
                        <div>
                            <b>Secret Code:</b> {selected.secretCode}
                        </div>
                        <div>
                            <b>Reports Made:</b> {selected.totalReportsMade}
                        </div>
                        <div>
                            <b>Reports Received:</b> {selected.totalReportsReceived}
                        </div>
                        <div>
                            <b>Risk Level:</b> {selected.riskLevel}
                        </div>
                        <div>
                            <b>Recruit Score:</b> {selected.recruitScore}
                        </div>
                        <div>
                            <b>Threat Score:</b> {selected.threatScore}
                        </div>
                        <div>
                            <b>Network Centrality:</b> {selected.networkCentrality}
                        </div>
                        <div>
                            <b>Influence Score:</b> {selected.influenceScore}
                        </div>
                        <div>
                            <b>Created At:</b> {new Date(selected.createdAt).toLocaleString()}
                        </div>
                        <div>
                            <b>Updated At:</b> {new Date(selected.updatedAt).toLocaleString()}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
