export class InnerResourcesPnModel {
  total: number;
  innerResourceList: Array<InnerResourcePnModel> = [];
  name: string;
}

export class InnerResourcePnModel {
  id: number;
  name: string;
  relatedOuterResourcesIds: Array<number> = [];
}
