import React, { useEffect, useState } from "react";
import { getPeople, createPerson, deletePerson } from "../Services/api";

const roleLabels = {
    0: "Agent",
    1: "Terrorist",
    2: "Intel",
    3: "Civilian",
    Agent: "Agent",
    Terrorist: "Terrorist",
    Intel: "Intel",
    Civilian: "Civilian",
};

export default function PeopleList() {
    const [people, setPeople] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [form, setForm] = useState({ name: "", secretCode: "", role: "0" });
    const [formError, setFormError] = useState("");
    const [formLoading, setFormLoading] = useState(false);

    useEffect(() => {
        fetchPeople();
    }, []);

    const fetchPeople = () => {
        setLoading(true);
        getPeople()
            .then(setPeople)
            .catch(setError)
            .finally(() => setLoading(false));
    };

    const handleOpenModal = () => {
        setForm({ name: "", secretCode: "", role: "0" });
        setFormError("");
        setShowModal(true);
    };

    const handleCloseModal = () => {
        setShowModal(false);
    };

    const handleFormChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();
        setFormError("");
        setFormLoading(true);
        try {
            await createPerson({
                name: form.name,
                secretCode: form.secretCode,
                role: parseInt(form.role, 10),
            });
            setShowModal(false);
            fetchPeople();
        } catch (err) {
            setFormError(err.message || "Failed to create person");
        } finally {
            setFormLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this person?")) return;
        try {
            await deletePerson(id);
            fetchPeople();
        } catch (err) {
            setError(err.message || "Failed to delete person");
        }
    };

    return (
        <div className="container mt-4">
            <div className="card shadow-sm">
                <div className="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                    <h4 className="mb-0">All Individuals</h4>
                    <button className="btn btn-success btn-sm" onClick={handleOpenModal}>
                        + Add New
                    </button>
                </div>
                <div className="card-body">
                    {loading && <div className="alert alert-info">Loading people...</div>}
                    {error && <div className="alert alert-danger">{error.message}</div>}
                    <ul className="list-group">
                        {people.map((p) => (
                            <li
                                key={p.Id}
                                className="list-group-item d-flex justify-content-between align-items-center"
                            >
                                <span>
                                    <span className="fw-bold">{p.Name}</span>{" "}
                                    <span className="badge bg-secondary ms-2">
                                        {roleLabels[p.Role] || "Unknown"}
                                    </span>
                                    <span className="text-muted ms-2">(ID: {p.Id})</span>
                                </span>
                                <span>
                                    <span className="text-muted me-3">Code: {p.SecretCode}</span>
                                    <button
                                        className="btn btn-outline-danger btn-sm"
                                        onClick={() => handleDelete(p.Id)}
                                    >
                                        Delete
                                    </button>
                                </span>
                            </li>
                        ))}
                    </ul>
                    <div className="alert alert-info">
                        <strong>Note:</strong> Risk Level and Recruit Score are
                        automatically computed from reports and cannot be set manually.
                    </div>
                </div>
            </div>
            {/* Modal for creating a new person */}
            {showModal && (
                <div
                    className="modal fade show d-block"
                    tabIndex="-1"
                    style={{ background: "rgba(0,0,0,0.3)" }}
                >
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Add New Individual</h5>
                                <button
                                    type="button"
                                    className="btn-close"
                                    onClick={handleCloseModal}
                                ></button>
                            </div>
                            <form onSubmit={handleFormSubmit}>
                                <div className="modal-body">
                                    {formError && (
                                        <div className="alert alert-danger">{formError}</div>
                                    )}
                                    <div className="mb-3">
                                        <label className="form-label">Name</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            name="name"
                                            value={form.name}
                                            onChange={handleFormChange}
                                            required
                                        />
                                    </div>
                                    <div className="mb-3">
                                        <label className="form-label">Secret Code</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            name="secretCode"
                                            value={form.secretCode}
                                            onChange={handleFormChange}
                                            required
                                        />
                                    </div>
                                    <div className="mb-3">
                                        <label className="form-label">Role</label>
                                        <select
                                            className="form-select"
                                            name="role"
                                            value={form.role}
                                            onChange={handleFormChange}
                                            required
                                        >
                                            <option value="0">Agent</option>
                                            <option value="1">Terrorist</option>
                                            <option value="2">Intel</option>
                                            <option value="3">Civilian</option>
                                        </select>
                                    </div>
                                </div>
                                <div className="modal-footer">
                                    <button
                                        type="button"
                                        className="btn btn-secondary"
                                        onClick={handleCloseModal}
                                    >
                                        Cancel
                                    </button>
                                    <button
                                        type="submit"
                                        className="btn btn-success"
                                        disabled={formLoading}
                                    >
                                        {formLoading ? "Saving..." : "Save"}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
