import { ActionRoute } from "./action-route.model";

export class MedicalControlActionConfig {
	public route = "";
	public queryParams = "";
	private actions = [
		{
			id: null,
			route: "",
			name: ""
		},
		{
			id: 1,
			route: `/action/absence`,
			name: "actionabsence"
		},
		{
			id: 2,
			route: `/action/absence`,
			name: "actionabsence"
		},
		{
			id: 3,
			route: `/action/absence`,
			name: "actionabsence"
		},
		{
			id: 4,
			route: `/action/absence`,
			name: "actionabsence"
		},
		{
			id: 6,
			route: `/action/break`,
			name: "actionbreak"
		},
		{
			id: 7,
			route: `/action/virtual-attention`,
			name: "actionvirtualattention"
		}
	];

	getActionById(id: number, idMedicalControl: number): Promise<ActionRoute> {
		const action = this.actions[0];
		let element;
		return new Promise((resolve) => {
			this.getRoute(idMedicalControl)
				.then(() => {
					element = this.actions.find((x) => x.id === id);
					action.route = `${this.route}${element.route}`;
					resolve(action);
				})
				.catch(() => {
					action.route = `${this.route}`;
					resolve(action);
				});
		});
	}

	getRoute(id: number): Promise<string> {
		return new Promise((resolve) => {
			this.route = "/healthApp/medical-control/create";
			if (id) {
				this.route = `/healthApp/medical-control/${id}`;
			}
			resolve("ok");
		});
	}
}
